using Blazored.Toast.Services;
using BlogSite.Client.Utility;
using BlogSite.Shared.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace BlogSite.Client.Shared
{
    public class SignInModalBase: ComponentBase
    {
        [Inject]
        protected HttpClient HttpClient { get; set; }
        [Inject]
        protected NavigationManager NavManager { get; set; }
        [Inject]
        protected IToastService ToastService { get; set; }
        [Inject] 
        protected AuthenticationStateProvider AuthStateProvider { get; set; }
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }
        protected bool IsSignUpMode { get; set; }
        protected string FirstName { get; set; } = string.Empty;
        protected string LastName { get; set; } = string.Empty;
        protected string Email { get; set; } = string.Empty;
        protected string Password { get; set; } = string.Empty;
        protected string ConfirmPassword { get; set; } = string.Empty;
        protected string ErrorMessage { get; set; } = string.Empty;
        protected bool IsLoading { get; set; }
        protected void ToggleSignUpMode()
        {
            ResetForm();
            IsSignUpMode = !IsSignUpMode;
        }

        protected async Task SubmitForm()
        {
            if(ValidateForm() == false) return;
            IsLoading = true;
            StateHasChanged();

            try
            {
                if (IsSignUpMode)
                {
                    var registerVm = new RegisterViewModel
                    {
                        FirstName = FirstName,
                        LastName = LastName,
                        Email = Email,
                        Password = Password,
                        ConfirmPassword = ConfirmPassword
                    };
                    var response = await HttpClient.PostAsJsonAsync<RegisterViewModel>("api/Account/Register", registerVm);
                    if (response != null && response.IsSuccessStatusCode)
                    {
                        ToastService.ShowSuccess("Registration successful. Try login now");
                        IsSignUpMode = false;
                    }
                }
                else
                {
                    var loginVm = new LoginViewModel
                    {
                        Username = Email,
                        Password = Password
                    };
                    var response = await HttpClient.PostAsJsonAsync<LoginViewModel>("api/Account/Login", loginVm);
                    if (response != null && response.IsSuccessStatusCode)
                    {
                        var userSession = await response.Content.ReadFromJsonAsync<AppUserViewModel>();
                        var custAuthSateProvider = (CustomAuthStateProvider)AuthStateProvider;
                        await custAuthSateProvider.UpdateAuthenticationState(userSession);
                        ResetForm();
                        await JSRuntime.InvokeVoidAsync("hideModal", "signInModal");
                        NavManager.NavigateTo("/", true);
                    }
                    if((response != null && response.StatusCode == System.Net.HttpStatusCode.Unauthorized))
                    {
                        ErrorMessage = "Username or password is incorrect";
                    }
                }
            }
            catch (Exception ex)
            {
                ToastService.ShowError("Something went wrong");
            }
            IsLoading = false;
            StateHasChanged();
        }

        private bool ValidateForm()
        {
            if (IsSignUpMode)
            {
                if (string.IsNullOrWhiteSpace(FirstName)|| string.IsNullOrWhiteSpace(LastName))
                {
                    ErrorMessage = "First & Last Name are required";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword))
                {
                    ErrorMessage = "Password & Confirm Password are required";
                    return false;
                }
                if (Password != ConfirmPassword)
                {
                    ErrorMessage = "Passwords don't match";
                    return false;
                }
            }
            if (string.IsNullOrWhiteSpace(Email)|| Regex.IsMatch(Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$") == false)
            {
                ErrorMessage = "A valid email is required";
                return false;
            }
            if (Regex.IsMatch(Password,@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*\W)(?!.*\s).{8,}$") == false)
            {
                ErrorMessage = "Password must contain at least one uppercase, one lowercase, one number and one special character";
                return false;
            }
            ErrorMessage = string.Empty;
            return true;
        }

        private void ResetForm()
        {
            ErrorMessage = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            Password = string.Empty;
            ConfirmPassword = string.Empty;
            Email = string.Empty;
        }
    }
}
