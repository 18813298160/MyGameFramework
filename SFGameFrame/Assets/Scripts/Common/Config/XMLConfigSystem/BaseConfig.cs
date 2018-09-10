public class BaseConfig
{
	public string id = "";
    public string path;
}

public class GuideProgressConfig : BaseConfig
{
    public string curFile = "";
    public int curIndex = 0;
    public int isFinish = 0;
}

public class GuideFileConfig : BaseConfig
{
	public string btns = "";
}

public class BaseAttrConfig : BaseConfig
{
	public int coin = 0;
	public int mana = 0;
	public int blood = 0;
	public int exp = 0;
}


