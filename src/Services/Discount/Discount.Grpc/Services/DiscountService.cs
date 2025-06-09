using Discount.Grpc.Data;
using Discount.Grpc.Models;
using Grpc.Core;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Discount.Grpc.Services;

public class DiscountService(DiscountContext dbContext, ILogger<DiscountService> logger)
    : DiscountProtoService.DiscountProtoServiceBase
{
    public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
    {
        var coupon = await dbContext.Coupons.FirstOrDefaultAsync(c => c.ProductName == request.ProductName);

        if (coupon is null)
            coupon = new Coupon { ProductName = "No Discount", Amount = 0, Description = "No Discount" };

        logger.LogInformation("Retrieved discount for product: {ProductName}", request.ProductName);

        return coupon.Adapt<CouponModel>();
    }

    public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
    {
        var coupon = request.Coupon.Adapt<Coupon>();
        return await SaveCouponAsync(coupon, isUpdate: false);
    }

    public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
    {
        var coupon = request.Coupon.Adapt<Coupon>();
        return await SaveCouponAsync(coupon, isUpdate: true);
    }

    public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
    {
        var coupon = await dbContext.Coupons.FirstOrDefaultAsync(c => c.ProductName == request.ProductName);

        if (coupon is null)
            throw new RpcException(new Status(StatusCode.NotFound, "Coupon not found"));

        dbContext.Coupons.Remove(coupon);

        try
        {
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Deleted discount for product: {ProductName}", request.ProductName);

            return new DeleteDiscountResponse { Success = true };
        }
        catch (DbUpdateException dbEx)
        {
            logger.LogError(dbEx, "Database update exception during deletion of product: {ProductName}", request.ProductName);
            throw new RpcException(new Status(StatusCode.Internal, "Failed to delete the coupon due to a database error."));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during deletion of product: {ProductName}", request.ProductName);
            throw new RpcException(new Status(StatusCode.Unknown, "An unexpected error occurred while deleting the coupon."));
        }
    }

    private async Task<CouponModel> SaveCouponAsync(Coupon coupon, bool isUpdate)
    {
        if (coupon is null)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid coupon data"));

        if (isUpdate)
        {
            dbContext.Coupons.Update(coupon);
            logger.LogInformation("Updated discount for product: {ProductName}", coupon.ProductName);
        }
        else
        {
            dbContext.Coupons.Add(coupon);
            logger.LogInformation("Created discount for product: {ProductName}", coupon.ProductName);
        }

        await dbContext.SaveChangesAsync();
        return coupon.Adapt<CouponModel>();
    }
}