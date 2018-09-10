using UnityEngine;

//继承MonoBehaviour的单例模板类
public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
	private static T _instance;
	public static T instance
	{
		get
		{
			if (_instance == null)
			{
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    GameObject go = new GameObject(typeof(T).Name);
                    _instance = go.AddComponent<T>();
                }
			}
			return _instance;
		}
	}
}

public class Single<T> where T : class, new()
{
	private static T _instance;
	public static T instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new T();
			}
			return _instance;
		}
	}
}