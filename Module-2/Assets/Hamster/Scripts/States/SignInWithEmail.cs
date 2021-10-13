using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;

namespace Hamster.States
{
    // State for requesting username/password from the user for
    // signing in with a preregistered email account.
    class SignInWithEmail : BaseState
    {
        FirebaseAuth auth;
        Menus.SignInGUI dialogComponent;
        bool canceled = false;

        public override void Initialize()
        {
            auth = FirebaseAuth.DefaultInstance;
            dialogComponent = SpawnUI<Menus.SignInGUI>(StringConstants.PrefabsSignInMenu);
        }

        public override void Suspend()
        {
            HideUI();
        }

        public override void Resume(StateExitValue results)
        {
            ShowUI();
            if (results != null)
            {
                if (results.sourceState == typeof(WaitForTask))
                {
                    WaitForTask.Results taskResults = results.data as WaitForTask.Results;
                    if (taskResults.task.IsFaulted)
                    {
                        manager.PushState(new BasicDialog(
                            Utilities.StringHelper.SigninInFailureString(taskResults.task)));
                    }
                    else
                    {
                        manager.PopState();
                    }
                }
            }
        }

        public override StateExitValue Cleanup()
        {
            DestroyUI();
            return new StateExitValue(typeof(SignInWithEmail), new SignInResult(canceled));
        }

        public override void HandleUIEvent(GameObject source, object eventData)
        {
            if (source == dialogComponent.CancelButton.gameObject)
            {
                canceled = true;
                manager.PopState();
            }
            else if (source == dialogComponent.ContinueButton.gameObject)
            {
                auth.SignInWithEmailAndPasswordAsync(dialogComponent.Email.text, dialogComponent.Password.text);
                //WRITE HERE THE CODE TO SIGN IN WITH AN EMAIL AND PASSWORD
                //AFTER USER CLICKS ON THE CONTINUE BUTTON
                Debug.Log("ATTEMPTED TO SIGN IN WITH AN ACCOUNT - THIS FUNCTION STILL NEEDS TO BE IMPLEMENTED");
            }
            else if (source == dialogComponent.ForgotPasswordButton.gameObject)
            {
                manager.PushState(new PasswordReset());
            }
        }
    }
}