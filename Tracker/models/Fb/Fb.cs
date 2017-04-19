using Facebook;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracker.Common;
using Windows.ApplicationModel.Activation;
using Windows.Security.Authentication.Web;

namespace Tracker.models.Fb
{
    class Fb 
    {
        FacebookClient _fb = new FacebookClient();
        readonly Uri _callbackUri = WebAuthenticationBroker.GetCurrentApplicationCallbackUri();
        readonly Uri _loginUrl;
        private const string FacebookAppId = "366652487050120";//Enter your FaceBook App ID here  
        private const string FacebookPermissions = "user_about_me";


        private object _defaultPostparams = new { client_id = FacebookAppId, display = "popup" };

        public Fb()
        {
          
        }
   
        public void share(PostParams postParams)
        {
            var postParamsMerged = Common.Common.Combine(postParams, _defaultPostparams);
            var url = _fb.GetDialogUrl("feed", postParamsMerged);

            try
            {
                WebAuthenticationBroker.AuthenticateAndContinue(url);
            }
            catch (Exception ex)
            {

            }
        }


    }


}
