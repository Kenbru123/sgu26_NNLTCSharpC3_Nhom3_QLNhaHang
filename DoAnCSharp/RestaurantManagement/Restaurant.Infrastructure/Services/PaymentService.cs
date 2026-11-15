using Microsoft.EntityFrameworkCore;
using Restaurant.Application.DTOs;
using Restaurant.Application.Interfaces;
using Restaurant.Domain.Entities;
using Restaurant.Infrastructure.Data;

namespace Restaurant.Infrastructure.Services;

public sealed class PaymentService : IPaymentService
{
    private readonly RestaurantDbContext _context;

    public PaymentService(RestaurantDbContext context)
    {
        _context = context;
    }

    public async Task<HoaDonDto?> GetInvoiceAsync(
        int orderId,
        CancellationToken cancellationToken)
    {
        var invoice = await _context.HoaDons
            .AsNoTracking()
            .SingleOrDefaultAsync(
                item => item.MaDonHang == orderId,
                cancellationToken);

        if (invoice is null)
        {
            return null;
        }

        return new HoaDonDto(
            invoice.MaHoaDon,
            invoice.MaDonHang,
            invoice.MaNhanVienLap,
            invoice.ThoiGianLap,
            invoice.ThoiGianThanhToan,
            invoice.TienHang,
            invoice.PhiGiaoChot,
            invoice.TyLeVAT,
            invoice.TienVAT,
            invoice.TongThanhToan,
            invoice.PhuongThucThanhToan,
            invoice.TrangThaiThanhToan);
    }

    public async Task<HoaDonDto> CreateOrUpdateInvoiceAsync(
        int orderId,
        int employeeId,
        PaymentInvoiceRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.PhuongThucThanhToan))
        {
            throw new ArgumentException("Vui lòng chọn phương thức thanh toán.");
        }

        var paymentMethod = request.PhuongThucThanhToan.Trim();
        if (paymentMethod is not ("TienMat" or "ChuyenKhoan" or "The"))
        {
            throw new ArgumentException("Phương thức thanh toán không hợp lệ.");
        }

        if (request.TyLeVAT is < 0 or > 100 ||
            decimal.Round(request.TyLeVAT, 2) != request.TyLeVAT)
        {
            throw new ArgumentException(
                "VAT phải nằm trong khoảng 0-100% và có tối đa 2 chữ số thập phân.");
        }

        var order = await _context.DonHangs
            .SingleOrDefaultAsync(
                item => item.MaDonHang == orderId,
                cancellationToken);

        if (order is null)
        {
            throw new KeyNotFoundException("Đơn hàng không tồn tại.");
        }

        var hasItems = await _context.ChiTietDonHangs
            .AnyAsync(item => item.MaDonHang == orderId, cancellationToken);

        if (!hasItems)
        {
            throw new InvalidOperationException("Không thể thanh toán đơn chưa có món.");
        }

        var subtotal = await _context.ChiTietDonHangs
            .Where(item => item.MaDonHang == orderId)
            .SumAsync(
                item => (decimal?)(item.SoLuong * item.DonGia),
                cancellationToken) ?? 0m;

        var vatAmount = decimal.Round(
            subtotal * request.TyLeVAT / 100m,
            2,
            MidpointRounding.AwayFromZero);

        var invoice = await _context.HoaDons
            .SingleOrDefaultAsync(
                item => item.MaDonHang == orderId,
                cancellationToken);

        if (invoice is null)
        {
            invoice = new HoaDon
            {
                MaDonHang = orderId,
                MaNhanVienLap = employeeId,
                ThoiGianLap = DateTime.UtcNow
            };
            _context.HoaDons.Add(invoice);
        }

        var now = DateTime.UtcNow;
        invoice.MaNhanVienLap = employeeId;
        invoice.ThoiGianLap = invoice.ThoiGianLap == default
            ? now
            : invoice.ThoiGianLap;
        invoice.ThoiGianThanhToan = now;
        invoice.TienHang = subtotal;
        invoice.PhiGiaoChot = 0m;
        invoice.TyLeVAT = request.TyLeVAT;
        invoice.TienVAT = vatAmount;
        invoice.TongThanhToan = subtotal + vatAmount;
        invoice.PhuongThucThanhToan = paymentMethod;
        invoice.TrangThaiThanhToan = "DaThanhToan";

        order.TrangThaiDon = "DaThanhToan";
        if (order.LoaiDon == "OnlineGiaoHang" &&
            string.IsNullOrWhiteSpace(order.TrangThaiGiao))
        {
            order.TrangThaiGiao = "ChoLayHang";
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new HoaDonDto(
            invoice.MaHoaDon,
            invoice.MaDonHang,
            invoice.MaNhanVienLap,
            invoice.ThoiGianLap,
            invoice.ThoiGianThanhToan,
            invoice.TienHang,
            invoice.PhiGiaoChot,
            invoice.TyLeVAT,
            invoice.TienVAT,
            invoice.TongThanhToan,
            invoice.PhuongThucThanhToan,
            invoice.TrangThaiThanhToan);
    }

    public async Task<HoaDonDto?> UpdateDeliveryStatusAsync(
        int orderId,
        string trangThaiGiao,
        CancellationToken cancellationToken)
    {
        var validStatuses = new[]
        {
            "ChoLayHang",
            "DangGiao",
            "DaGiao",
            "DaNhan",
            "HoanTat"
        };

        var normalized = trangThaiGiao?.Trim();
        if (string.IsNullOrWhiteSpace(normalized) ||
            !validStatuses.Contains(normalized))
        {
            throw new ArgumentException("Trạng thái giao hàng không hợp lệ.");
        }

        var order = await _context.DonHangs
            .SingleOrDefaultAsync(
                item => item.MaDonHang == orderId,
                cancellationToken);

        if (order is null)
        {
            return null;
        }

        if (order.LoaiDon != "OnlineGiaoHang")
        {
            throw new ArgumentException(
                "Chỉ đơn hàng giao tận nơi mới có thể cập nhật trạng thái giao hàng.");
        }

        order.TrangThaiGiao = normalized;
        if (normalized is "DaNhan" or "HoanTat")
        {
            order.TrangThaiDon = "HoanTat";
        }

        await _context.SaveChangesAsync(cancellationToken);
        return await GetInvoiceAsync(orderId, cancellationToken);
    }
}
