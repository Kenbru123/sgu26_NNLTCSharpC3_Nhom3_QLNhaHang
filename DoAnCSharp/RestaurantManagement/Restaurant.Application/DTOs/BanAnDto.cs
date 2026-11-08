namespace Restaurant.Application.DTOs;

/// <summary>Table details returned by the restaurant API.</summary>
public sealed record BanAnDto(
    int MaBan,
    int MaKhuVuc,
    string TenKhuVuc,
    string TenBan,
    int SoCho,
    bool TrangThaiHoatDong);
