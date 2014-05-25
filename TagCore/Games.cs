using System;
using System.Collections;

namespace FreeAllegiance.Tag
{
	/// <summary>
	/// A collection of Game objects
	/// </summary>
	public class Games : IEnumerable, IList
	{
		private ArrayList	_games;

		/// <summary>
		/// Default Constructor
		/// </summary>
		public Games ()
		{
			_games = new ArrayList();
		}

		/// <summary>
		/// Returns an enumerator that can iterate through the Games within this collection
		/// </summary>
		/// <returns>An enumerator that iterates through this collection</returns>
		public IEnumerator GetEnumerator ()
		{
			return _games.GetEnumerator();
		}

		/// <summary>
		/// Retrieves the game with the specified ID from the list
		/// </summary>
		/// <param name="gameID">The GameID to search for</param>
		/// <returns>The found game, or null if it does not exist</returns>
		public Game GetGameByID (int gameID)
		{
			Game Result = null;

			foreach (Game TempGame in _games)
			{
				if (TempGame.GameID == gameID)
				{
					Result = TempGame;
					break;
				}
			}

			return Result;
		}

		/// <summary>
		/// Removes the specified game from the collection
		/// </summary>
		/// <param name="gameID"></param>
		public void RemoveGame (int gameID)
		{
			Game RemovedGame = GetGameByID(gameID);
			this.Remove(RemovedGame);
			RemovedGame.Dispose();
		}

		#region IList Members

		/// <summary>
		/// Gets a value indicating whether this games list is Read Only
		/// </summary>
		public bool IsReadOnly
		{
			get {return _games.IsReadOnly;}
		}

		/// <summary>
		/// Retrieves a game from this list
		/// </summary>
		public object this[int index]
		{
			get {return _games[index];}
			set {_games[index] = value;}
		}

		/// <summary>
		/// Removes the game at the specified index
		/// </summary>
		/// <param name="index">The index of the game to remove</param>
		public void RemoveAt (int index)
		{
			_games.RemoveAt(index);
		}

		/// <summary>
		/// Inserts a new game at the specified index
		/// </summary>
		/// <param name="index">The index at which to insert the specified game</param>
		/// <param name="value">The game to insert</param>
		public void Insert (int index, object value)
		{
			_games.Insert(index, value);
		}

		/// <summary>
		/// Removes the specified game from the collection
		/// </summary>
		/// <param name="value">The game to remove</param>
		public void Remove(object value)
		{
			_games.Remove(value);
		}

		/// <summary>
		/// Determines if the specified game exists in this collection
		/// </summary>
		/// <param name="value">The game to look for</param>
		/// <returns>True if the specified game is in this collection</returns>
		public bool Contains(object value)
		{
			return _games.Contains(value);
		}

		/// <summary>
		/// Clears this list of games
		/// </summary>
		public void Clear()
		{
			_games.Clear();
		}

		/// <summary>
		/// Retrieves the index of the specified game
		/// </summary>
		/// <param name="value">The game to look for</param>
		/// <returns>The index of the game if found</returns>
		public int IndexOf(object value)
		{
			return _games.IndexOf(value);
		}

		/// <summary>
		/// Adds the specified game to the collection
		/// </summary>
		/// <param name="value">The game to add to the collection</param>
		/// <returns>The index of the added game</returns>
		public int Add (Game value)
		{
			return Add((object)value);
		}

		/// <summary>
		/// Adds the specified game to the collection
		/// </summary>
		/// <param name="value">The game to add to the collection</param>
		/// <returns>The index of the added game</returns>
		public int Add(object value)
		{
			return _games.Add(value);
		}

		/// <summary>
		/// Gets a value determining if this list has a fixed size
		/// </summary>
		public bool IsFixedSize
		{
			get {return _games.IsFixedSize;}
		}

		#endregion

		#region ICollection Members

		/// <summary>
		/// Gets a value indicating whether access to the Games is synchronized (thread-safe)
		/// </summary>
		public bool IsSynchronized
		{
			get {return _games.IsSynchronized;}
		}

		/// <summary>
		/// The number of games in this collection
		/// </summary>
		public int Count
		{
			get {return _games.Count;}
		}

		/// <summary>
		/// Copies this games list to the specified array at the specified index
		/// </summary>
		/// <param name="array">The array to which this game should be copied</param>
		/// <param name="index">The index at which to start copying</param>
		public void CopyTo (Array array, int index)
		{
			_games.CopyTo(array, index);
		}

		/// <summary>
		/// Gets an object that can be used to synchronize access to this games list
		/// </summary>
		public object SyncRoot
		{
			get {return _games.SyncRoot;}
		}

		#endregion
	}
}
