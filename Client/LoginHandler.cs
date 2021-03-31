using System;
using API.Global;
using API.Profile;
using GM.Client.LoginHelpers;
using GM.Global;
using GM.InteractionSystem;
using GTANetworkAPI;

using Auth = API.Profile.LoginHandler;

namespace GM.Client
{
    public class LoginHandler : Script
    {
        [ServerEvent(Event.PlayerConnected)]
        public void PlayerConnected(Player player)
        {
            player.SetSharedData(PlayerVariables.GodMode, false);
            player.SetSharedData(PlayerVariables.Freezed, true);
            
            player.TriggerEvent("Player:SetAsLogged", false);

            player.SetInteract(null);
            player.SetSharedData(PlayerVariables.Visible, false);
            
            player.TriggerEvent("Player:Preload");
            player.SetSharedData(PlayerVariables.AccessLevel, 0);
            
            var doesAccountExist = Auth.DoesAccountExist(player.SocialClubName, player.SocialClubId);

            player.Dimension = (uint) Dimensions.Login + player.Id;
            
            player.TriggerEvent("Auth:Toggle", true, doesAccountExist);
            player.TriggerEvent("Auth:OpenPage", (int) (doesAccountExist != "null" ? PageName.SignIn : PageName.SignUp));
        }

        [RemoteEvent("Auth:TryRegister")]
        public void TryRegister(Player player, string username, string password, string promoCode)
        {
            if (Auth.DoesAccountExist(player.SocialClubName, player.SocialClubId) != "null")
            {
                player.TriggerEvent("Auth:OpenPage", (int) PageName.SignIn);
                return;
            }

            if (!Auth.DoesAccountCreate(player.SocialClubName, player.SocialClubId, username))
            {
                player.TriggerEvent("Auth:SetError", FieldName.Username, ErrorName.WrongLogin);
                return;
            }
            
            player.SetData(PlayerVariables.RegisterAccount, new TemporaryRegisterData()
            {
                Email = string.Empty,
                Login = username,
                Password = password,
                Promo = promoCode
            });
            
            // Auth.CreateAccount(username, password, player.SocialClubName, player.SocialClubId);
            // player.ShowNotify(Notify.MessageType.Success, "Success!", $"Вы успешно зарегестрировали аккаунт {username}");
            player.TriggerEvent("Auth:OpenPage", (int) PageName.EmailConfirm);
        }

        [RemoteEvent("Auth:TryLogin")]
        public void TryLogin(Player player, string username, string password)
        {
            if (Auth.DoesAccountExist(player.SocialClubName, player.SocialClubId) == "null")
            {
                player.TriggerEvent("Auth:OpenPage", (int) PageName.SignUp);
                return;
            }

            switch (Auth.CanLogin(player.SocialClubName, player.SocialClubId, username, password))
            {
                case LoginErrorCode.Username:
                    player.TriggerEvent("Auth:SetError", FieldName.Username, ErrorName.WrongLogin);
                    return;
                case LoginErrorCode.Password:
                    player.TriggerEvent("Auth:SetError", FieldName.Password, ErrorName.WrongPassword);
                    return;
                case LoginErrorCode.Success:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            var account = Auth.LoadAccount(player.SocialClubName, player.SocialClubId, username, password);
            player.SetData(PlayerVariables.Account, account);
            
            CharactersHandler.Upload(player);
        }

        [RemoteEvent("Auth:DoesEmailValid")]
        public void DoesEmailValid(Player player, string email)
        {
            if (!player.HasData(PlayerVariables.RegisterAccount))
            {
                player.TriggerEvent("Auth:OpenPage", (int) PageName.SignUp);
                return;
            }
            
            if (Auth.CheckEmailValid(email))
            {
                player.TriggerEvent("Auth:SetError", FieldName.EmailCode, ErrorName.ExistEmail);
                return;
            }

            var data = player.GetData<TemporaryRegisterData>(PlayerVariables.RegisterAccount);
            data.Email = email;
            
            // player.TriggerEvent("Auth:SetError", FieldName.EmailCode, ErrorName.NoError);
            SetEmailConfirmationCode(player, email);
        }
        
        [RemoteEvent("Auth:DoesRestoreEmailValid")]
        public void DoesRestoreEmailValid(Player player, string email)
        {
            if (!Auth.CheckEmailValid(email))
            {
                player.TriggerEvent("Auth:SetError", FieldName.Email, ErrorName.WrongEmail);
                return;
            }

            // player.TriggerEvent("Auth:SetError", FieldName.EmailCode, ErrorName.NoError);
            SetEmailConfirmationCode(player, email);
        }
        
        [RemoteEvent("Auth:SendCode")]
        public void SendEmailCode(Player player, string email)
        {
            if (!Auth.CheckEmailValid(email))
            {
                player.TriggerEvent("Auth:SetError", FieldName.EmailCode, ErrorName.WrongEmailCode);
                return;
            }

            SetEmailConfirmationCode(player, email);
        }

        private static void SetEmailConfirmationCode(Player player, string email)
        {
            var code = CodeGenerator.Next();
  
            var sendResult = Email.Send(email, "Email confirmation code", code);
            if (sendResult)
            {
                player.SetData("EMAIL_CODE", code);
                player.ShowNotify(Notify.MessageType.Success, "Email", $"Сообщение отправлено на {email}", 5000);
            }
            else
            {
                player.ShowNotify(Notify.MessageType.Alert, "Email", $"Ошибка при отправке сообщения на {email}. Попробуйте позже!", 5000);
            }
            
            player.TriggerEvent("Auth:ActivateEmailConfirmation");
        }

        [RemoteEvent("Auth:CheckRegisterCode")]
        public void CheckCode(Player player, string code)
        {
            if(!player.HasData("EMAIL_CODE")) {
                player.TriggerEvent("Auth:OpenPage", (int) PageName.EmailConfirm);
                return;
            }

            if (!player.HasData(PlayerVariables.RegisterAccount)) {
                player.TriggerEvent("Auth:OpenPage", (int) PageName.SignUp);
                return;
            }

            var original = player.GetData<string>("EMAIL_CODE");
            if (code != original)
            {
                player.TriggerEvent("Auth:SetError", FieldName.EmailCode, ErrorName.WrongEmailCode);
                return;
            }

            var data = player.GetData<TemporaryRegisterData>(PlayerVariables.RegisterAccount);

            Auth.CreateAccount(data.Login, data.Password, data.Email, player.SocialClubName, player.SocialClubId);
            player.ShowNotify(Notify.MessageType.Success, "Success!", $"Вы успешно зарегестрировали аккаунт {data.Login}");

            player.ResetData(PlayerVariables.RegisterAccount);
            player.TriggerEvent("Auth:OpenPage", (int) PageName.SignIn);
        }
        
        [RemoteEvent("Auth:CheckRestoreCode")]
        public void CheckRestoreCode(Player player, string code)
        {
            if(!player.HasData("EMAIL_CODE")) {
                player.TriggerEvent("Auth:OpenPage", (int) PageName.SignIn);
                return;
            }
            
            var original = player.GetData<string>("EMAIL_CODE");
            if (code != original)
            {
                player.TriggerEvent("Auth:SetError", FieldName.EmailCode, ErrorName.WrongEmailCode);
                return;
            }

            player.TriggerEvent("Auth:OpenPage", (int) PageName.ChangePass);
        }

        [RemoteEvent("Auth:ChangePassword")]
        public void ChangePassword(Player player, string password)
        {
            if (Auth.DoesAccountExist(player.SocialClubName, player.SocialClubId) == "null")
            {
                player.TriggerEvent("Auth:OpenPage", (int) PageName.SignUp);
                return;
            }
            
            Auth.ChangePassword(player.SocialClubName, player.SocialClubId, password);
            
            player.ShowNotify(Notify.MessageType.Success, "Success!", $"Пароль был обновлен!");
            player.TriggerEvent("Auth:OpenPage", (int) PageName.SignIn);
        }
    }
}