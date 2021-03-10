using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public GameObject[] starObjs;
	public int maxRow = 12;
	public int maxColumn = 10;

	public GameObject starGroup;
	/// <summary>
	/// 当前所有星星集合
	/// </summary>
	public List<Star> StarList;
	/// <summary>
	/// 当前点击星星相邻且颜色相同的星星集合;中间变量，用完记得清理
	/// </summary>
	public List<Star> ClickedStarList;

	//Static 
	public static GameManager gameManager_Instance;

	//分数
	public Text currentScoreText;
	public float currentScore = 0f;
	public Text currentTotalScoreText;
	public float currentTotalScore = 0f;

	public Text HurdleText;
	public int HurdleIndex = 1;
	public Text targetScoreText;
	public float targetScore = 0f;

	public int judgeSwitch = 0;
	public Button replay;

	//音效
	public AudioSource clearSource;
	public AudioSource bgMusicSource;

	public GameObject[] particles;
	// Use this for initialization
	void Start () {
		gameManager_Instance = this;
		GameStart(1);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	/// <summary>
	/// 开始游戏
	/// </summary>
	void GameStart(int hurdle)
	{
		if (bgMusicSource != null)
		{
			bgMusicSource.Play();
		}
		replay.gameObject.SetActive(false);//启用/禁用

		HurdleIndex = hurdle;
		currentScore = 0;
		currentTotalScore = 0;

		SetHurdleTargetScore(HurdleIndex);
		CreateStarList();

		judgeSwitch = 0;
	}

	void CreateStarList()
    {
        //Instantiate();//实例化方法1、Object，2、position 3、rotation
		//row*Column 个星星
        for (int r=0;r< maxRow; r++)
        {
            for (int c=0;c< maxColumn; c++)
            {
				//row,column
				int index = Random.Range(0,5);//0 Blue,1 Green,2 Orange,3 Purple,4 Red
				var obj = Instantiate(starObjs[index],starGroup.transform);
				//r行,c列
				Vector3 pos = new Vector3(48*c,48*r,0);
				//obj.transform.position 世界坐标
				//obj.transform.localPosition 相对于父对象而言的坐标
				obj.transform.localPosition = pos;

				var star = obj.GetComponent<Star>();
				star.Row = r;//行
				star.Column = c;//列
				StarList.Add(star);
			}
        }
    }
	/// <summary>
	/// 递归寻找CurrentStar相邻且颜色相同的星星集合，并存到ClickedStarList集合中
	/// </summary>
	/// <param name="currentStar"></param>
	public void FindTheSameStar(Star currentStar)
    {
		int row = currentStar.Row;
		int column = currentStar.Column;
        //Up row+1,column
        if (row<maxRow)
		{
			foreach (var item in StarList)
			{
				if (item.Row == row + 1 && item.Column == column)
				{
					//item==Up star
					if (item.starColor == currentStar.starColor)
					{
                        if (!ClickedStarList.Contains(item))
						{
							ClickedStarList.Add(item);
							FindTheSameStar(item);
						}
					}
				}
			}
		}
		//Down row-1,column
		if (row > 0)
		{
			foreach (var item in StarList)
			{
				if (item.Row == row - 1 && item.Column == column)
				{
					//item==Up star
					if (item.starColor == currentStar.starColor)
					{
						if (!ClickedStarList.Contains(item))
						{
							ClickedStarList.Add(item);
							FindTheSameStar(item);
						}
					}
				}
			}
		}

		//Left row,column-1
		if (column>0)
		{
			foreach (var item in StarList)
			{
				if (item.Row == row && item.Column == column-1)
				{
					//item==Up star
					if (item.starColor == currentStar.starColor)
					{
						if (!ClickedStarList.Contains(item))
						{
							ClickedStarList.Add(item);
							FindTheSameStar(item);
						}
					}
				}
			}
		}
		//Right row,column+1
		if (column < maxColumn)
		{
			foreach (var item in StarList)
			{
				if (item.Row == row && item.Column == column+1)
				{
					//item==Up star
					if (item.starColor == currentStar.starColor)
					{
						if (!ClickedStarList.Contains(item))
						{
							ClickedStarList.Add(item);
							FindTheSameStar(item);
						}
					}
				}
			}
		}

	}

	/// <summary>
	/// 销毁ClickedStarList集合中的星星，并且从StarList中移除ClickedStarList
	/// </summary>
	public void ClearClickedStarList()
    {
        if (ClickedStarList.Count>=2)
        {

            //销毁
            foreach (var item in ClickedStarList)
            {
				//添加粒子特效
				int colorIndex = (int)item.starColor;
                if (particles.Length>=colorIndex)
				{
					GameObject parObj = particles[colorIndex];
					var obj = Instantiate(parObj, starGroup.transform);
					obj.transform.localPosition = item.transform.localPosition;
				}

				item.DestroyStar();
				StarList.Remove(item);
            }
            //向下移动
            foreach(var restStar in StarList)
            {
				int moveCount = 0;
				//计算需要向下移动的行数
                foreach (var clickedStar in ClickedStarList)
                {
                    if (restStar.Column== clickedStar.Column&& restStar.Row> clickedStar.Row)
                    {
						moveCount++;
					}
                }
                if (moveCount>0)
				{
					restStar.moveDownCount = moveCount;
					restStar.OpenMoveDown();
				}
			}


            //向左平移;按列遍历遍历
            for (int col=maxColumn-2;col>=0;col--)
            {
				bool IsEmpty = true;
				//判断列是否为空
                foreach (var restStar in StarList)
                {
                    if (restStar.Column==col)
                    {
						IsEmpty = false;
					}
                }
                if (IsEmpty)
				{
					//计算左移的列数
					foreach (var restStar in StarList)
                    {
						//位于空列的右边
                        if (restStar.Column>col)
                        {
							restStar.moveLeftCount++;
						}
                    }
				}
            }
			//找到moveLeftCount>0的星星，打开左移的开关
			foreach (var restStar in StarList)
            {
                if (restStar.moveLeftCount>=1)
                {
					restStar.OpenMoveLeft();
                }
            }

            //音效
            if (clearSource!=null)
			{
				clearSource.Play();
			}
        }

		//分数计算
		CalculateScore(ClickedStarList.Count);

		ClickedStarList.Clear();

		JudgeHurdleOver();
	}

	public void JudgeHurdleOver()
    {
		bool isOver = true;
        foreach (var restStar in StarList)
        {
			FindTheSameStar(restStar);
            if (ClickedStarList.Count>0)
            {
				isOver = false;
			}
		}
		ClickedStarList.Clear();

		if (isOver)
		{

			if (judgeSwitch == 0)
			{
				judgeSwitch = 1;

				//Hurdle Over
				Debug.Log("Over");
				RestStarRewardScore(StarList.Count);
                foreach (var restStar in StarList)
                {
					restStar.DestroyStar();
				}
				StarList.Clear();

				//判断总得分是否超过目标分？是：进入下一关；否：游戏结束
				if (currentTotalScore >= targetScore)
				{
					//Next Hurdle
					int index = HurdleIndex + 1;
					GameStart(index);//重新加载关卡
				}
				else
				{
					//GameOver
					GameOver();
				}
			}
		}

	}

	/// <summary>
	/// 计算分数
	/// </summary>
	/// <param name="destoryStarCount"></param>
	void CalculateScore(int destoryStarCount)
    {
        if (destoryStarCount>=2)
        {
			float tempScore = 0f;
            for (int i=0;i<destoryStarCount;i++)
            {
				tempScore += i * 10 + 5;
			}
			currentScore = tempScore;
			currentScoreText.text = currentScore.ToString();//Text显示
			currentTotalScore += currentScore;
			currentTotalScoreText.text = currentTotalScore.ToString();//总分显示
		}
    }
	/// <summary>
	/// 剩余星星奖励分数
	/// </summary>
	/// <param name="restStarCount"></param>
	void RestStarRewardScore(int restStarCount)
    {
		float rewardScore = 0;
        if (restStarCount<10)
        {
			rewardScore = 2000 - restStarCount * 100;
			currentTotalScore += rewardScore;
			currentTotalScoreText.text = currentTotalScore.ToString();
		}

    }
	void SetHurdleTargetScore(int hurdleIndex)
    {
		HurdleText.text = "关卡："+hurdleIndex.ToString();

		if (hurdleIndex==1)
        {
			targetScore = 1000;
        }
        else if(hurdleIndex>1)
        {
			targetScore = 1000;
			for (int i= 1;i<hurdleIndex;i++)
            {
				targetScore += 2000 + (i - 1) * 100;
			}
        }
		targetScoreText.text = "目标："+targetScore.ToString();
    }

	void GameOver()
    {
		replay.gameObject.SetActive(true);//启用
	}
	public void ReplayGame()
    {
		SceneManager.LoadScene("PopStar");
    }
}
