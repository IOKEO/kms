using Azure.Core;
using Azure.Identity;
using Microsoft.Graph;
using Microsoft.TeamsFx;

namespace Kms.Services
{
    public class GraphClientService
    {
        //les variables nécessaires 
        public string ErrorMessage { get; set; }
        private readonly IConfiguration _configuration;
        private readonly TeamsUserCredential _teamsUserCredential;

        //Initialisation des variables utiles
        public GraphClientService(IConfiguration configuration, TeamsUserCredential teamsUserCredential)
        {
            _configuration = configuration;
            _teamsUserCredential = teamsUserCredential;
        }
        //la fonction permet de verifié préalablement si pour le scope définit l'utilisateur à les permission nécessaire
        //Si non la fonction retourne false
        public async Task<bool> HasPermission(string scope)
        {
            try
            {
                var tokenCredential = await GetOnBehalfOfCredential();
                await tokenCredential.GetTokenAsync(new TokenRequestContext(new string[] { scope }), new CancellationToken());

                return true;
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
                return false;
            }

        }

        //La fonction permet de créer un token d'authenfication a partir de l'id du tenant, du client secret et d'un token d'accès 
        public async Task<OnBehalfOfCredential> GetOnBehalfOfCredential()
        {
            var config = _configuration.Get<ConfigOptions>();
            var tenantId = config.TeamsFx.Authentication.OAuthAuthority.Remove(0, "https://login.microsoftonline.com/".Length);
            AccessToken ssoToken = await _teamsUserCredential.GetTokenAsync(new TokenRequestContext(null), new CancellationToken());
            return new OnBehalfOfCredential(
                tenantId,
                config.TeamsFx.Authentication.ClientId,
                config.TeamsFx.Authentication.ClientSecret,
                ssoToken.Token
            );
        }

        //Le client graph permettant d'envoyer des requetes à l'API est crée en utilisant le token d'authentification
        public GraphServiceClient GetGraphServiceClient(string scope, TokenCredential tokenCredential)
        {
            var client = new GraphServiceClient(tokenCredential, new string[] { scope });
            return client;
        }
    }
}
