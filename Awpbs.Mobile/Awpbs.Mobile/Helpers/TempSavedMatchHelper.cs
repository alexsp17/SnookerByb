using System;

namespace Awpbs.Mobile
{
	/// <summary>
	/// Saves the match score on the keychain, just in case.
	/// Only able to save&restore a single match.
	/// </summary>
	public class TempSavedMatchHelper
	{
		public const string KeyChainName = "LastMatch";
		
		IKeyChain keyChain;
		
		public TempSavedMatchHelper(IKeyChain keyChain)
		{
			this.keyChain = keyChain;
		}

		public void Save(SnookerMatchScore match)
		{
			try
			{
				Score score = new Score ();
				match.PostToScore (score);
				score.IsUnfinished = true;
				string json = Newtonsoft.Json.JsonConvert.SerializeObject (score);
				keyChain.Add (KeyChainName, json);
			}
			catch (Exception)
			{
			}
		}

		public void Remove()
		{
			try
			{
				keyChain.Delete (KeyChainName);
			}
			catch (Exception)
			{
			}
		}

		public void SaveToDbAndRemove(Repository repository)
		{
			var score = loadScore();
			if (score == null)
				return;
			score.IsUnfinished = true;
			if (score.Guid == null || score.Guid == Guid.Empty)
			{
				score.Guid = Guid.NewGuid ();
				score.Date = DateTimeHelper.GetUtcNow ();
				score.TimeModified = score.Date;
			}

			try
			{
				Score existingScore = null;
				if (score.ScoreID > 0)
					existingScore = repository.GetScore(score.ScoreID);
				if (existingScore == null)
				{
					repository.AddScore(score);
				}
				else
				{
					repository.UpdateScore(score);
				}
			}
			catch (Exception)
			{
			}

			Remove ();
		}

		private Score loadScore()
		{
			try
			{
				string json = keyChain.Get (KeyChainName);
				if (string.IsNullOrEmpty (json))
					return null;
				Score score = Newtonsoft.Json.JsonConvert.DeserializeObject<Score> (json);
				return score;
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}

