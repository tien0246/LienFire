using System;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms.Impl;

namespace UnityEngine.SocialPlatforms;

public class Local : ISocialPlatform
{
	private static LocalUser m_LocalUser;

	private List<UserProfile> m_Friends = new List<UserProfile>();

	private List<UserProfile> m_Users = new List<UserProfile>();

	private List<AchievementDescription> m_AchievementDescriptions = new List<AchievementDescription>();

	private List<Achievement> m_Achievements = new List<Achievement>();

	private List<Leaderboard> m_Leaderboards = new List<Leaderboard>();

	private Texture2D m_DefaultTexture;

	public ILocalUser localUser
	{
		get
		{
			if (m_LocalUser == null)
			{
				m_LocalUser = new LocalUser();
			}
			return m_LocalUser;
		}
	}

	void ISocialPlatform.Authenticate(ILocalUser user, Action<bool> callback)
	{
		LocalUser localUser = (LocalUser)user;
		m_DefaultTexture = CreateDummyTexture(32, 32);
		PopulateStaticData();
		localUser.SetAuthenticated(value: true);
		localUser.SetUnderage(value: false);
		localUser.SetUserID("1000");
		localUser.SetUserName("Lerpz");
		localUser.SetImage(m_DefaultTexture);
		callback?.Invoke(obj: true);
	}

	void ISocialPlatform.Authenticate(ILocalUser user, Action<bool, string> callback)
	{
		((ISocialPlatform)this).Authenticate(user, (Action<bool>)delegate(bool success)
		{
			callback(success, null);
		});
	}

	void ISocialPlatform.LoadFriends(ILocalUser user, Action<bool> callback)
	{
		if (VerifyUser())
		{
			LocalUser obj = (LocalUser)user;
			IUserProfile[] friends = m_Friends.ToArray();
			obj.SetFriends(friends);
			callback?.Invoke(obj: true);
		}
	}

	public void LoadUsers(string[] userIDs, Action<IUserProfile[]> callback)
	{
		List<UserProfile> list = new List<UserProfile>();
		if (!VerifyUser())
		{
			return;
		}
		foreach (string text in userIDs)
		{
			foreach (UserProfile user in m_Users)
			{
				if (user.id == text)
				{
					list.Add(user);
				}
			}
			foreach (UserProfile friend in m_Friends)
			{
				if (friend.id == text)
				{
					list.Add(friend);
				}
			}
		}
		IUserProfile[] obj = list.ToArray();
		callback(obj);
	}

	public void ReportProgress(string id, double progress, Action<bool> callback)
	{
		if (!VerifyUser())
		{
			return;
		}
		foreach (Achievement achievement in m_Achievements)
		{
			if (achievement.id == id && achievement.percentCompleted <= progress)
			{
				if (progress >= 100.0)
				{
					achievement.SetCompleted(value: true);
				}
				achievement.SetHidden(value: false);
				achievement.SetLastReportedDate(DateTime.Now);
				achievement.percentCompleted = progress;
				callback?.Invoke(obj: true);
				return;
			}
		}
		foreach (AchievementDescription achievementDescription in m_AchievementDescriptions)
		{
			if (achievementDescription.id == id)
			{
				bool completed = progress >= 100.0;
				Achievement item = new Achievement(id, progress, completed, hidden: false, DateTime.Now);
				m_Achievements.Add(item);
				callback?.Invoke(obj: true);
				return;
			}
		}
		Debug.LogError("Achievement ID not found");
		callback?.Invoke(obj: false);
	}

	public void LoadAchievementDescriptions(Action<IAchievementDescription[]> callback)
	{
		if (VerifyUser() && callback != null)
		{
			IAchievementDescription[] obj = m_AchievementDescriptions.ToArray();
			callback(obj);
		}
	}

	public void LoadAchievements(Action<IAchievement[]> callback)
	{
		if (VerifyUser() && callback != null)
		{
			IAchievement[] obj = m_Achievements.ToArray();
			callback(obj);
		}
	}

	public void ReportScore(long score, string board, Action<bool> callback)
	{
		if (!VerifyUser())
		{
			return;
		}
		foreach (Leaderboard leaderboard in m_Leaderboards)
		{
			if (leaderboard.id == board)
			{
				List<Score> list = new List<Score>((Score[])leaderboard.scores);
				list.Add(new Score(board, score, localUser.id, DateTime.Now, score + " points", 0));
				IScore[] scores = list.ToArray();
				leaderboard.SetScores(scores);
				callback?.Invoke(obj: true);
				return;
			}
		}
		Debug.LogError("Leaderboard not found");
		callback?.Invoke(obj: false);
	}

	public void LoadScores(string leaderboardID, Action<IScore[]> callback)
	{
		if (!VerifyUser())
		{
			return;
		}
		foreach (Leaderboard leaderboard in m_Leaderboards)
		{
			if (leaderboard.id == leaderboardID)
			{
				SortScores(leaderboard);
				callback?.Invoke(leaderboard.scores);
				return;
			}
		}
		Debug.LogError("Leaderboard not found");
		if (callback != null)
		{
			IScore[] obj = new Score[0];
			callback(obj);
		}
	}

	void ISocialPlatform.LoadScores(ILeaderboard board, Action<bool> callback)
	{
		if (!VerifyUser())
		{
			return;
		}
		Leaderboard leaderboard = (Leaderboard)board;
		foreach (Leaderboard leaderboard2 in m_Leaderboards)
		{
			if (leaderboard2.id == leaderboard.id)
			{
				leaderboard.SetTitle(leaderboard2.title);
				leaderboard.SetScores(leaderboard2.scores);
				leaderboard.SetMaxRange((uint)leaderboard2.scores.Length);
			}
		}
		SortScores(leaderboard);
		SetLocalPlayerScore(leaderboard);
		callback?.Invoke(obj: true);
	}

	bool ISocialPlatform.GetLoading(ILeaderboard board)
	{
		if (!VerifyUser())
		{
			return false;
		}
		return ((Leaderboard)board).loading;
	}

	private void SortScores(Leaderboard board)
	{
		List<Score> list = new List<Score>((Score[])board.scores);
		list.Sort((Score s1, Score s2) => s2.value.CompareTo(s1.value));
		for (int num = 0; num < list.Count; num++)
		{
			list[num].SetRank(num + 1);
		}
	}

	private void SetLocalPlayerScore(Leaderboard board)
	{
		IScore[] scores = board.scores;
		for (int i = 0; i < scores.Length; i++)
		{
			Score score = (Score)scores[i];
			if (score.userID == localUser.id)
			{
				board.SetLocalUserScore(score);
				break;
			}
		}
	}

	public void ShowAchievementsUI()
	{
		Debug.Log("ShowAchievementsUI not implemented");
	}

	public void ShowLeaderboardUI()
	{
		Debug.Log("ShowLeaderboardUI not implemented");
	}

	public ILeaderboard CreateLeaderboard()
	{
		return new Leaderboard();
	}

	public IAchievement CreateAchievement()
	{
		return new Achievement();
	}

	private bool VerifyUser()
	{
		if (!localUser.authenticated)
		{
			Debug.LogError("Must authenticate first");
			return false;
		}
		return true;
	}

	private void PopulateStaticData()
	{
		m_Friends.Add(new UserProfile("Fred", "1001", friend: true, UserState.Online, m_DefaultTexture));
		m_Friends.Add(new UserProfile("Julia", "1002", friend: true, UserState.Online, m_DefaultTexture));
		m_Friends.Add(new UserProfile("Jeff", "1003", friend: true, UserState.Online, m_DefaultTexture));
		m_Users.Add(new UserProfile("Sam", "1004", friend: false, UserState.Offline, m_DefaultTexture));
		m_Users.Add(new UserProfile("Max", "1005", friend: false, UserState.Offline, m_DefaultTexture));
		m_AchievementDescriptions.Add(new AchievementDescription("Achievement01", "First achievement", m_DefaultTexture, "Get first achievement", "Received first achievement", hidden: false, 10));
		m_AchievementDescriptions.Add(new AchievementDescription("Achievement02", "Second achievement", m_DefaultTexture, "Get second achievement", "Received second achievement", hidden: false, 20));
		m_AchievementDescriptions.Add(new AchievementDescription("Achievement03", "Third achievement", m_DefaultTexture, "Get third achievement", "Received third achievement", hidden: false, 15));
		Leaderboard leaderboard = new Leaderboard();
		leaderboard.SetTitle("High Scores");
		leaderboard.id = "Leaderboard01";
		List<Score> list = new List<Score>();
		list.Add(new Score("Leaderboard01", 300L, "1001", DateTime.Now.AddDays(-1.0), "300 points", 1));
		list.Add(new Score("Leaderboard01", 255L, "1002", DateTime.Now.AddDays(-1.0), "255 points", 2));
		list.Add(new Score("Leaderboard01", 55L, "1003", DateTime.Now.AddDays(-1.0), "55 points", 3));
		list.Add(new Score("Leaderboard01", 10L, "1004", DateTime.Now.AddDays(-1.0), "10 points", 4));
		IScore[] scores = list.ToArray();
		leaderboard.SetScores(scores);
		m_Leaderboards.Add(leaderboard);
	}

	private Texture2D CreateDummyTexture(int width, int height)
	{
		Texture2D texture2D = new Texture2D(width, height);
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				Color color = (((j & i) > 0) ? Color.white : Color.gray);
				texture2D.SetPixel(j, i, color);
			}
		}
		texture2D.Apply();
		return texture2D;
	}
}
