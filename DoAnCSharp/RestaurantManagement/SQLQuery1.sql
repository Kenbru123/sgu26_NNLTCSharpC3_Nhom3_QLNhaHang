USE RestaurantDB;
GO

INSERT INTO DanhMucMon (TenDanhMuc, MoTa)
VALUES
(N'Món chính', N'Các món ?n chính'),
(N'?? u?ng', N'Các lo?i n??c u?ng'),
(N'Tráng mi?ng', N'Các món tráng mi?ng');
INSERT INTO MonAn
(
    TenMon,
    MoTa,
    DonGia,
    HinhAnh,
    TrangThai,
    MaDanhMuc
)
VALUES
(
    N'Bò lúc l?c',
    N'Bò xào rau c?',
    105000,
    NULL,
    1,
    1
),
(
    N'Mì Ý bò b?m',
    N'Mì Ý s?t bò b?m',
    89000,
    NULL,
    1,
    1
),
(
    N'Coca Cola',
    N'N??c ng?t có ga',
    20000,
    NULL,
    1,
    2
);
SELECT * FROM DanhMucMon;
SELECT * FROM MonAn;