using System.Collections.Generic;
using Landfall.TABS;
using Landfall.TABS.GameState;
using TFBGames;
using UnityEngine;
using UnityEngine.Events;
using Landfall.TABS.Workshop;

public class SecretUnlockDragon : GameStateListener
{
	[SerializeField]
	private string m_secret_key = "";

	public TABSCampaignAsset m_unlockCampaign;

	[SerializeField]
	private float m_distanceToUnlock = 5f;

	private RotationShake m_rotationShake;

	private Rigidbody m_secretObject;

	private float m_lookValue;

	private float m_unlockValue;

	public AudioClip hitClip;

	private AudioSource loopSource;

	private Transform m_mainCamTransform;

	public UnityEvent unlockEvent;

	public UnityEvent hideEvent;

	private bool done;

	public Color glowColor;

	public GameObject unlockSparkEffect;

	protected override void Awake()
	{
		base.Awake();
		if (m_mainCamTransform == null)
		{
			OnEnterNewScene();
		}
	}

	private void Update()
	{
		if (!(m_mainCamTransform != null) || !m_secretObject || done)
		{
			return;
		}
		loopSource.volume = Mathf.Pow(m_unlockValue * 0.25f, 1.3f);
		var pitch = 1f + 1f * m_unlockValue;
		loopSource.pitch = pitch >= 0f ? pitch : 0f;
		if (m_unlockValue > 0f || m_lookValue > 10f)
		{
			SetColor();
		}
		float num = Vector3.Distance(m_secretObject.worldCenterOfMass, m_mainCamTransform.position);
		if (num > m_distanceToUnlock)
		{
			m_unlockValue -= Time.unscaledDeltaTime * 0.2f;
			return;
		}
		float num2 = Vector3.Angle(m_mainCamTransform.forward, m_secretObject.worldCenterOfMass - m_mainCamTransform.position);
		m_lookValue = 1000f / (num * num2);
		if (m_lookValue > 8f)
		{
			float num3 = 0.2f;
			m_unlockValue += num3 * Time.unscaledDeltaTime;
			UnlockProgressFeedback();
			if (m_unlockValue > 1f)
			{
				UnlockSecret();
			}
		}
		else
		{
			m_unlockValue -= Time.unscaledDeltaTime * 0.2f;
		}
	}

	private void UnlockProgressFeedback()
	{
		if ((bool)m_rotationShake)
		{
			if (m_unlockValue <= 0f)
			{
				m_rotationShake.AddForce(Random.onUnitSphere * 2f);
				m_unlockValue = 0f;
			}
			m_rotationShake.enabled = true;
			m_rotationShake.AddForce(Random.onUnitSphere * m_unlockValue * Time.deltaTime * 50f);
		}
	}

	private void SetColor()
	{
		m_unlockValue = Mathf.Clamp(m_unlockValue, 0f, float.PositiveInfinity);
		Renderer[] componentsInChildren = m_secretObject.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Material[] materials = componentsInChildren[i].materials;
			for (int j = 0; j < materials.Length; j++)
			{
				if (materials[j].HasProperty("_EmissionColor"))
				{
					materials[j].EnableKeyword("_EMISSION");
					materials[j].SetColor("_EmissionColor", glowColor * m_unlockValue * 2f);
				}
			}
			componentsInChildren[i].materials = materials;
		}
	}

	private void UnlockSecret()
	{
		if (!base.enabled)
		{
			return;
		}
		if ((bool)ScreenShake.Instance)
		{
			ScreenShake.Instance.AddForce(Vector3.up * 8f, m_secretObject.transform.position);
		}
		if ((bool)unlockSparkEffect)
		{
			GameObject gameObject = Object.Instantiate(unlockSparkEffect, m_secretObject.transform.position, m_secretObject.transform.rotation);
			gameObject.AddComponent<RemoveAfterSeconds>().seconds = 5f;
			MeshRenderer componentInChildren = m_secretObject.GetComponentInChildren<MeshRenderer>();
			if ((bool)componentInChildren)
			{
				ParticleSystem.ShapeModule shape = gameObject.GetComponent<ParticleSystem>().shape;
				shape.meshRenderer = componentInChildren;
			}
		}
		m_secretObject.gameObject.SetActive(false);
		unlockEvent?.Invoke();
		loopSource.Stop();
		loopSource.volume = 1f;
		loopSource.PlayOneShot(hitClip);
		done = true;
		List<Faction> factioniests = new List<Faction>();
		foreach (var facts in LandfallUnitDatabase.GetDatabase().FactionList)
		{
			if (facts.Entity.Name.Contains("Medieval"))
			{
				factioniests.Add(LandfallUnitDatabase.GetDatabase().GetFactionByGUID(facts.Entity.GUID));
			}
			if (facts.Entity.Name.Contains("Farmer"))
			{
				factioniests.Add(LandfallUnitDatabase.GetDatabase().GetFactionByGUID(facts.Entity.GUID));
			}
		}
		m_unlockCampaign.LevelsInCampaign[0].AllowedFactions = factioniests.ToArray();
		m_unlockCampaign.LevelsInCampaign[0].MapAsset = LandfallUnitDatabase.GetDatabase().GetMap(new DatabaseID(-1, 835120575));
		CampaignPlayerDataHolder.StartedPlayingNewCampaign(m_unlockCampaign, 0);
		TABSSceneManager.LoadCampaign();
		LandfallUnitDatabase.GetDatabase().AddCampaignWithID(m_unlockCampaign);
		if (string.IsNullOrWhiteSpace(m_secret_key))
		{
			return;
		}
		ServiceLocator.GetService<ISaveLoaderService>().UnlockSecret(m_secret_key);
		CheckAchievements();
		//ServiceLocator.GetService<ModalPanel>().OpenUnlockPanel(m_secretDescription, m_secretIcon);
		//if (list != null && list.Count > 0)
		//{
		//	foreach (SecretUnlockCondition item in list)
		//	{
		//		ServiceLocator.GetService<ModalPanel>().OpenUnlockPanel(item.m_unlockDescription, item.m_unlockImage);
		//	}
		//}
		PlacementUI placementUI = Object.FindObjectOfType<PlacementUI>();
		if (placementUI != null)
		{
			placementUI.RedrawUI(m_secret_key);
		}
	}

	public override void OnEnterNewScene()
	{
		base.OnEnterNewScene();
		loopSource = GetComponent<AudioSource>();
		if ((bool)loopSource)
		{
			loopSource.volume = 0f;
		}
		m_rotationShake = GetComponentInChildren<RotationShake>();
		m_secretObject = GetComponentInChildren<Rigidbody>();
		if ((bool)m_secretObject)
		{
			m_secretObject.isKinematic = true;
		}
		//if (!string.IsNullOrWhiteSpace(m_secret_key) && ServiceLocator.GetService<ISaveLoaderService>().HasUnlockedSecret(m_secret_key))
		//{
		//	if ((bool)m_secretObject)
		//	{
		//		m_secretObject.gameObject.SetActive(false);
		//	}
		//	base.enabled = false;
		//	hideEvent?.Invoke();
		//}
		MainCam mainCam = ServiceLocator.GetService<PlayerCamerasManager>()?.GetMainCam(TFBGames.Player.One);
		m_mainCamTransform = ((mainCam != null) ? mainCam.transform : null);
	}

	public override void OnEnterPlacementState()
	{
	}

	public override void OnEnterBattleState()
	{
	}

	private void CheckAchievements()
	{
		AchievementService service = ServiceLocator.GetService<AchievementService>();
		ISaveLoaderService secretService = ServiceLocator.GetService<ISaveLoaderService>();
		if (HasUnlockedFaction(874593522))
		{
			service.UnlockAchievement("UNLOCKED_ALL_SECRET");
		}
		if (HasUnlockedFaction(673578412))
		{
			service.UnlockAchievement("UNLOCKED_ALL_LEGACY");
		}
		bool HasUnlockedFaction(int factionId)
		{
			UnitBlueprint[] units = LandfallUnitDatabase.GetDatabase().GetFactionByGUID(new DatabaseID(-1, factionId)).Units;
			foreach (UnitBlueprint unitBlueprint in units)
			{
				if (!string.IsNullOrEmpty(unitBlueprint.Entity.UnlockKey) && !secretService.HasUnlockedSecret(unitBlueprint.Entity.UnlockKey))
				{
					return false;
				}
			}
			return true;
		}
	}
}
