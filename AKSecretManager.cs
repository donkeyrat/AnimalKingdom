using UnityEngine;
using UnityEngine.SceneManagement;

namespace AnimalKingdom
{
    public class AKSecretManager
    {
        public AKSecretManager()
        {
            SceneManager.sceneLoaded += SceneLoaded;
        }

        public void SceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene.name == "05_Lvl1_Medieval_VC")
            {
                var secrets = new GameObject()
                {
                    name = "Secrets"
                };
                Object.Instantiate(AKMain.kermate.LoadAsset<GameObject>("SecretUnlock_Dragon"), secrets.transform, true);
            }
        }
	}
}
