// Copyright 2017 Google Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
//using Firebase.Unity.Editor;

namespace Hamster.States
{
    // State for requesting user sign-in details.
    class CreateAccount : BaseState
    {
        Menus.CreateAccountGUI dialogComponent;
        bool canceled = false;
        FirebaseAuth auth;

        public override void Initialize()
        {
            auth = FirebaseAuth.DefaultInstance;
            dialogComponent = SpawnUI<Menus.CreateAccountGUI>(StringConstants.PrefabsNewAccountMenu);
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
                        manager.PushState(new BasicDialog("Could not create account."));
                    }
                    else
                    {
                        UserProfile userProfile = new UserProfile();
                        userProfile.DisplayName = dialogComponent.DisplayName.text;
                        auth.CurrentUser.UpdateUserProfileAsync(userProfile);
                        //We got the results, time to go back to the menu
                        manager.PopState();
                    }
                }
            }
        }

        public override void Suspend()
        {
            HideUI();
        }

        public override StateExitValue Cleanup()
        {
            DestroyUI();
            return new StateExitValue(typeof(CreateAccount), new SignInResult(canceled));
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
                auth.CreateUserWithEmailAndPasswordAsync(dialogComponent.Email.text, dialogComponent.Password.text);
                //WRITE HERE THE CODE TO CREATE AN ACCOUNT WITH
                //EMAIL AND PASSWORD AFTER THEY CLICK ON THE CONTINUE BUTTON
                Debug.Log("ATTEMPTED TO CREATE AN ACCOUNT - THIS FUNCTION STILL NEEDS TO BE IMPLEMENTED");
            }
        }
    }
}
