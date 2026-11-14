namespace Restaurant.API.Authentication;

public static class RestaurantRoles
{
    public const string Manager = "QuanLy,Admin";
    public const string Employee = "NhanVien";
    public const string ManagerOrEmployee = Manager + "," + Employee;
}
