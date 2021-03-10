using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//星星颜色枚举
public enum StarColor
{
	Blue =0,
	Green =1,
	Orange = 2,
	Purple = 3,
	Red = 4,
}
public class Star : MonoBehaviour {
	//Public声明的变量/对象，可以在Unity Inspector视图中可视化查看以及修改
	public int Row = 0;
	public int Column = 0;
	public StarColor starColor = StarColor.Blue;

	public int moveDownCount = 0;
	private bool IsMoveDown = false;
	public float speed = -2f;

	public int moveLeftCount = 0;
	private bool IsMoveLeft = false;
	private int targetRow = 0;
	private int targetColumn = 0;

	// Use this for initialization
	//游戏对象启用的时候，执行一次
	void Start () {
		
	}
	
	// Update is called once per frame
	//Unity更新方法，每帧执行一次，每帧的时间间隔可以修改
	void Update () {
		//向下移动
        if (IsMoveDown)
		{
			Row = targetRow;
			Vector3 downVector = new Vector3(0, 1, 0);
			//当前的Y坐标位置为row*48；需要移动的距离是48*moveDownCount;最终移动到达的位置为row*48-48*moveDownCount
			if (this.transform.localPosition.y>targetRow *48f)
            {
				this.transform.Translate(downVector * speed*Time.deltaTime);//移动
			}
			//会有一定时间的延迟
            else
			{
				this.transform.localPosition = new Vector3(this.transform.localPosition.x, targetRow * 48f, this.transform.localPosition.z);
				IsMoveDown = false;
				moveDownCount = 0;
			}
		}

		//向左移动
		if (IsMoveLeft)
		{
			Column = targetColumn;
			Vector3 downVector = new Vector3(1, 0, 0);
			//当前的X坐标位置为Column*48；需要移动的距离是48*moveLeftCount;最终移动到达的位置为(Column - moveLeftCount) * 48f
			if (this.transform.localPosition.x > targetColumn * 48f)
			{
				this.transform.Translate(downVector * speed*Time.deltaTime);
			}
			else
			{
				this.transform.localPosition = new Vector3( targetColumn * 48f, this.transform.localPosition.y, this.transform.localPosition.z);
				IsMoveLeft = false;
				moveLeftCount = 0;
			}
		}


	}
	public void OnClick_Star()
    {
		//Debug.Log(starColor.ToString());
		GameManager.gameManager_Instance.FindTheSameStar(this);
		GameManager.gameManager_Instance.ClearClickedStarList();
    }

	/// <summary>
	/// 销毁自身
	/// </summary>
	public void DestroyStar()
    {
		Destroy(this.gameObject);
    }
	/// <summary>
	/// 向下移动的开关
	/// </summary>
	public void OpenMoveDown()
    {
		IsMoveDown = true;
		targetRow = Row - moveDownCount;
	}
	public void OpenMoveLeft()
    {
		IsMoveLeft = true;
		targetColumn = Column - moveLeftCount;

	}
	//public void MoveDown()
	//   {

	//   }

}
