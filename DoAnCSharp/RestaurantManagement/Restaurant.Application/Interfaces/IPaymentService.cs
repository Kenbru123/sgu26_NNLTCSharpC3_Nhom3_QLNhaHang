using Restaurant.Application.DTOs;

namespace Restaurant.Application.Interfaces;

public interface IPaymentService
{
    Task<HoaDonDto?> GetInvoiceAsync(int orderId, CancellationToken cancellationToken);

    Task<HoaDonDto> CreateOrUpdateInvoiceAsync(
        int orderId,
        int employeeId,
        PaymentInvoiceRequest request,
        CancellationToken cancellationToken);

    Task<HoaDonDto?> UpdateDeliveryStatusAsync(
        int orderId,
        string trangThaiGiao,
        CancellationToken cancellationToken);
}
