using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Application.DTOs.Account;
using Restaurant.Application.Interfaces;

namespace Restaurant.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "ManagementOnly")]
public sealed class AccountsController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountsController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet("employees")]
    public async Task<IActionResult> GetEmployees(CancellationToken cancellationToken)
    {
        // Lấy danh sách tài khoản được liên kết với nhân viên.
        var accounts = await _accountService.GetEmployeeAccountsAsync(cancellationToken);
        return Ok(accounts);
    }

    [HttpPost("employees")]
    public async Task<IActionResult> CreateEmployee(
        [FromBody] CreateEmployeeAccountRequest request,
        CancellationToken cancellationToken)
    {
        // Quản lý tạo hồ sơ nhân viên và tài khoản đăng nhập tương ứng.
        var result = await _accountService.CreateEmployeeAccountAsync(request, cancellationToken);
        if (!result.Succeeded)
        {
            return Conflict(new { message = result.Error });
        }

        return StatusCode(StatusCodes.Status201Created, result.Account);
    }

    [HttpPut("{accountId:int}/role")]
    public async Task<IActionResult> UpdateRole(
        int accountId,
        [FromBody] UpdateRoleRequest request,
        CancellationToken cancellationToken)
    {
        // Thay đổi vai trò của tài khoản nhân viên.
        var result = await _accountService.UpdateRoleAsync(accountId, request, cancellationToken);
        return ToActionResult(result);
    }

    [HttpPatch("{accountId:int}/status")]
    public async Task<IActionResult> UpdateStatus(
        int accountId,
        [FromBody] UpdateAccountStatusRequest request,
        CancellationToken cancellationToken)
    {
        // Khóa hoặc mở khóa tài khoản nhân viên.
        var result = await _accountService.UpdateStatusAsync(accountId, request, cancellationToken);
        return ToActionResult(result);
    }

    [HttpPut("{accountId:int}/password")]
    public async Task<IActionResult> ChangePassword(
        int accountId,
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        // Đổi mật khẩu và lưu lại dưới dạng hash.
        var result = await _accountService.ChangePasswordAsync(accountId, request, cancellationToken);
        return ToActionResult(result);
    }

    private IActionResult ToActionResult(AccountResult result)
    {
        if (result.Succeeded)
        {
            return Ok(result.Account);
        }

        // Trả 404 nếu không tìm thấy tài khoản, các lỗi khác trả 400.
        if (result.Error?.StartsWith("Không tìm thấy", StringComparison.Ordinal) == true)
        {
            return NotFound(new { message = result.Error });
        }

        return BadRequest(new { message = result.Error });
    }
}
