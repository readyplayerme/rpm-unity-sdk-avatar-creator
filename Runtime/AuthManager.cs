﻿using System;
using System.Threading.Tasks;
using ReadyPlayerMe.Core;
using UnityEngine;

namespace ReadyPlayerMe.AvatarCreator
{
    /// <summary>
    /// Provides methods for managing the user's authentication and session.
    /// </summary>
    public static class AuthManager
    {
        private const string TAG = nameof(AuthManager);
        private static readonly AuthenticationRequests AuthenticationRequests;
        private static UserSession userSession;
        public static UserSession UserSession => userSession;

        public static bool IsSignedIn;
        public static bool IsSignedInAnonymously;

        public static Action<UserSession> OnSignedIn;
        public static Action<UserSession> OnSessionRefreshed;
        public static Action OnSignedOut;
        public static Action<string> OnSignInError;

        static AuthManager()
        {
            AuthenticationRequests = new AuthenticationRequests(CoreSettingsHandler.CoreSettings.Subdomain);
        }

        public static async Task LoginAsAnonymous()
        {
            userSession = await AuthenticationRequests.LoginAsAnonymous();
            IsSignedInAnonymously = true;
        }

        public static void SetUser(UserSession session)
        {
            userSession = session;
            IsSignedIn = true;
            OnSignedIn?.Invoke(userSession);
        }

        public static async void SendEmailCode(string email)
        {
            await AuthenticationRequests.SendCodeToEmail(email, userSession.Id);
        }

        public static async Task<bool> LoginWithCode(string otp)
        {
            try
            {
                userSession = await AuthenticationRequests.LoginWithCode(otp);
                IsSignedIn = true;
                OnSignedIn?.Invoke(userSession);
                return true;
            }
            catch (Exception e)
            {
                OnSignInError?.Invoke(e.Message);
                return false;
            }
        }

        public static async void Signup(string email)
        {
            await AuthenticationRequests.Signup(email, userSession.Id);
        }

        public static async Task RefreshToken()
        {
            (string, string) newTokens;
            try
            {
                newTokens = await AuthenticationRequests.RefreshToken(userSession.Token, userSession.RefreshToken);
            }
            catch (Exception e)
            {
                SDKLogger.Log(TAG, "Refreshing token failed with error: " + e.Message);
                throw;
            }

            userSession.Token = newTokens.Item1;
            userSession.RefreshToken = newTokens.Item2;
            OnSessionRefreshed?.Invoke(userSession);
        }

        public static void Logout()
        {
            IsSignedIn = false;
            IsSignedInAnonymously = false;
            userSession = new UserSession();
            OnSignedOut?.Invoke();
        }
    }
}
