using System.Threading;

public class AutoTask : Auto
{
	public int LessOrEqualsTask;

	public string Phai;

	public static object objTask;

	public static bool isTask;

	public static int saveTD;

	public long bagsort;

	public long boxsort;

	public int useThucAn;

	public long timeuptn;

	public static object objSleep;

	public int taskId => Char.getMyChar().ctaskId;

	public int mapId => GameScr.gI().getTaskMapId();

	public int Npcid => GameScr.gI().getTaskNpcId();

	public int index => Char.getMyChar().taskMaint.index;

	public bool mainnull => Char.getMyChar().taskMaint == null;

	public Char me => Char.getMyChar();

	public override void Run()
	{
		if (Auto.isDie(Char.getMyChar()))
		{
			Hoisinh(isWait: true);
			Sleep(500);
			if (taskId > 8 && !isTruong() && TileMap.mapID != 27)
			{
				LuuToaDo();
			}
		}
		else
		{
			Actions.isThuLinh = false;
			Actions.isTinhAnh = false;
			TuDong();
			a();
		}
	}

	public override string ToString()
	{
		if (LessOrEqualsTask == -1)
		{
			return "Auto nhiệm vụ: " + Phai;
		}
		return "Auto nhiệm vụ: " + Phai + " < or = " + LessOrEqualsTask;
	}

	public void update(string Phai, int LessOrEqualsTask = -1)
	{
		Update();
		this.LessOrEqualsTask = LessOrEqualsTask;
		this.Phai = Phai;
	}

	public void Start()
	{
		if (taskId == 0)
		{
			if (Char.getMyChar().taskMaint == null)
			{
				Pick(Npcid, menu: false, getTask: true);
				Sleep(200);
				return;
			}
			if (index != 6)
			{
				for (int i = 0; i < Npc.arrNpcTemplate[Npcid].menu.Length; i++)
				{
					string text = Npc.arrNpcTemplate[Npcid].menu[i][0];
					if (text.Equals("Talk") || (text.Equals("Nói chuyện") && Npcid != 12))
					{
						Pick(Npcid, menu: true, getTask: false, i);
						Lock(500);
						return;
					}
				}
			}
			if (index == 6)
			{
				Pick(Npcid, menu: true);
				Lock();
				return;
			}
		}
		if (taskId == 1)
		{
			if (mainnull && taskId != 5)
			{
				Pick(Npcid, menu: false, getTask: true);
				Lock(1000);
				return;
			}
			if (index == 0)
			{
				Pick(Npcid, menu: false, getTask: true, 1);
				Lock(1000);
				return;
			}
			if (index == 1)
			{
				Pick(Npcid, menu: false, getTask: true);
				Lock(1000);
				return;
			}
			if (index == 2)
			{
				Pick(Npcid, menu: false, getTask: true, 1);
				Lock(1000);
				return;
			}
			if (index == 3)
			{
				Pick(Npcid, menu: false, getTask: true, 2);
				Lock(1000);
				return;
			}
			if (index == 4)
			{
				Pick(Npcid, menu: false, getTask: true);
				Lock(1000);
				return;
			}
			if (index == 5)
			{
				Pick(Npcid, menu: true);
				Lock(1000);
				return;
			}
		}
		if (taskId == 2)
		{
			if (mainnull)
			{
				Pick(Npcid, menu: false, getTask: true);
				Lock(1000);
				return;
			}
			if (index == 0)
			{
				if (ItemBagItemId(194) != null)
				{
					Service.gI().useItem(ItemBagItemId(194).indexUI);
					Lock(1000);
				}
				else
				{
					Pick(5, menu: true);
					Service.gI().itemBoxToBag(ItemBoxItemId(194).indexUI);
					Sleep();
				}
				return;
			}
			if (index == 1)
			{
				Attack(0, -1);
				return;
			}
			if (index == 2)
			{
				Pick(Npcid, menu: true);
				Lock(1000);
				return;
			}
		}
		if (taskId == 3)
		{
			if (mainnull)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
					return;
				}
				Pick(Npcid, menu: false, getTask: true);
				Lock(1000);
				return;
			}
			if (index == 0)
			{
				if (ItemBagItemId(23) == null)
				{
					Mua(idThucAn(), 2);
				}
				else
				{
					Mua(idThucAn(), 1);
				}
				return;
			}
			if (index == 1)
			{
				if (ItemBagItemId(23) != null)
				{
					Service.gI().useItem(ItemBagItemId(23).indexUI);
					Lock(1000);
				}
				else
				{
					S(2);
				}
				return;
			}
			if (index == 2)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
				}
				else
				{
					Attack(1, -1);
				}
				return;
			}
			if (index == 3)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
				}
				else
				{
					Attack(2, -1);
				}
				return;
			}
			if (index == 4)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
					return;
				}
				Pick(Npcid, menu: true);
				Lock(1000);
				return;
			}
		}
		if (taskId == 4)
		{
			if (mainnull)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
					return;
				}
				Pick(Npcid, menu: false, getTask: true);
				Lock(1000);
				return;
			}
			if (index == 0)
			{
				if (TileMap.mapID != 23)
				{
					Go(23);
				}
				else
				{
					Attack(1, -1);
				}
				return;
			}
			if (index == 1)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
					return;
				}
				PickItemNv();
				Attack(3, -1);
				return;
			}
			if (index == 2)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
					return;
				}
				PickItemNv();
				Attack(4, -1);
				return;
			}
			if (index == 3)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
					return;
				}
				Pick(Npcid, menu: true);
				Lock(1000);
				return;
			}
		}
		if (taskId == 5)
		{
			if (mainnull)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
					return;
				}
				Pick(Npcid, menu: false, getTask: true);
				Lock(1000);
				return;
			}
			if (index == 0)
			{
				if (TileMap.mapID != 23)
				{
					Go(23);
				}
				else
				{
					Attack(1, -1);
				}
				return;
			}
			if (index == 1)
			{
				if (TileMap.mapID != 20)
				{
					Go(20);
					return;
				}
				PickItemNv();
				Attack(54, -1);
				return;
			}
			if (index == 2)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
					return;
				}
				Pick(Npcid, menu: true);
				Lock(1000);
				return;
			}
		}
		if (taskId == 6)
		{
			if (mainnull)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
					return;
				}
				Pick(Npcid, menu: false, getTask: true);
				Lock(1000);
				return;
			}
			if (index == 0)
			{
				if (TileMap.mapID != 23)
				{
					Go(23);
				}
				else
				{
					Attack(-1, -1);
				}
				return;
			}
			if (index == 1)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
				}
				return;
			}
			if (index == 2)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
				}
				return;
			}
			if (index == 3)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
				}
				return;
			}
			if (index == 4)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
					return;
				}
				Pick(Npcid, menu: true);
				Lock(1000);
				return;
			}
		}
		if (taskId == 7)
		{
			if (mainnull)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
					return;
				}
				Pick(Npcid, menu: false, getTask: true);
				Lock(1000);
				return;
			}
			if (index == 0)
			{
				if (TileMap.mapID != 2)
				{
					Go(2);
				}
				else
				{
					Attack(-1, -1);
				}
				return;
			}
			if (index == 1)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
					return;
				}
				Pick(Npcid, menu: false, getTask: true, 1);
				Lock(1000);
				return;
			}
			if (index == 2)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
					return;
				}
				Pick(Npcid, menu: false, getTask: true);
				Lock(1000);
				return;
			}
			if (index == 3)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
					return;
				}
				Pick(Npcid, menu: false, getTask: true, 1);
				Lock(1000);
				return;
			}
			if (index == 4)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
					return;
				}
				Pick(Npcid, menu: false, getTask: true, 1);
				Lock(1000);
				return;
			}
			if (index == 5)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
					return;
				}
				Pick(Npcid, menu: false, getTask: true, 2);
				Lock(1000);
				return;
			}
			if (index == 6)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
					return;
				}
				Pick(Npcid, menu: false, getTask: true, 2);
				Lock(1000);
				return;
			}
			if (index == 7)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
					return;
				}
				Pick(Npcid, menu: false, getTask: true);
				Lock(1000);
				return;
			}
			if (index == 8)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
					return;
				}
				Pick(Npcid, menu: false, getTask: true, 2);
				Lock(1000);
				return;
			}
			if (index == 9)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
					return;
				}
				Pick(Npcid, menu: false, getTask: true, 2);
				Lock(1000);
				return;
			}
			if (index == 10)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
					return;
				}
				Pick(Npcid, menu: false, getTask: true, 1);
				Lock(1000);
				return;
			}
			if (index == 11)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
					return;
				}
				Pick(Npcid, menu: false, getTask: true);
				Lock(1000);
				return;
			}
			if (index == 12)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
					return;
				}
				Pick(Npcid, menu: false, getTask: true, 1);
				Lock(1000);
				return;
			}
			if (index == 13)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
					return;
				}
				Pick(Npcid, menu: false, getTask: true, 2);
				Lock(1000);
				return;
			}
			if (index == 14)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
					return;
				}
				Pick(Npcid, menu: false, getTask: true, 2);
				Lock(1000);
				return;
			}
			if (index == 15)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
					return;
				}
				Pick(Npcid, menu: false, getTask: true, 1);
				Lock(1000);
				return;
			}
			if (index == 16)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
					return;
				}
				Pick(Npcid, menu: true);
				Lock(1000);
				return;
			}
		}
		if (taskId == 8)
		{
			if (mainnull)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
					return;
				}
				Pick(Npcid, menu: false, getTask: true);
				Lock(1000);
				return;
			}
			if (index == 0)
			{
				if (TileMap.mapID != 2)
				{
					Go(2);
				}
				else
				{
					Attack(-1, -1);
				}
				return;
			}
			if (index == 1)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
					return;
				}
				Pick(Npcid, menu: true, getTask: false, 3);
				Lock(1000);
				return;
			}
			if (index == 2)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
					return;
				}
				Pick(Npcid, menu: true, getTask: false, 3);
				Lock(1000);
				return;
			}
			if (index == 3)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
					return;
				}
				Pick(Npcid, menu: true, getTask: false, 3);
				Lock(1000);
				return;
			}
			if (index == 4)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
					return;
				}
				Pick(Npcid, menu: true);
				Lock(1000);
				return;
			}
		}
		if (taskId == 9)
		{
			if (Char.getMyChar().nClass.classId == 0)
			{
				Vo();
				return;
			}
			if (mainnull)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
					return;
				}
				Pick(Npcid, menu: false, getTask: true);
				Lock(1000);
				return;
			}
			if (index == 0)
			{
				if (TileMap.mapID != 72)
				{
					Go(72);
					return;
				}
				LuuToaDo();
				SVC();
				return;
			}
			if (index == 1)
			{
				if (Char.getMyChar().sPoint > 0 && Char.getMyChar().clevel >= 11)
				{
					upSkillTask();
				}
				else if (TileMap.mapID != 28)
				{
					Go(28);
				}
				else
				{
					Attack(-1, -1);
				}
				return;
			}
			if (index == 2)
			{
				if (Char.getMyChar().pPoint <= 0)
				{
					if (TileMap.mapID != 28)
					{
						Go(28);
					}
					else
					{
						Attack(-1, -1);
					}
				}
				return;
			}
			if (index == 3)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
					return;
				}
				Pick(Npcid, menu: true);
				Lock(1000);
				return;
			}
		}
		if (taskId == 10)
		{
			if (mainnull)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
					return;
				}
				Pick(Npcid, menu: false, getTask: true);
				Lock(1000);
				return;
			}
			if (index == 0)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
				}
				else
				{
					Attack(5, -1);
				}
				return;
			}
			if (index == 1)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
				}
				else
				{
					Attack(6, -1);
				}
				return;
			}
			if (index == 2)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
				}
				else
				{
					Attack(7, -1);
				}
				return;
			}
			if (index == 3)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
					return;
				}
				Pick(Npcid, menu: true);
				Lock(1000);
				return;
			}
		}
		if (taskId == 11)
		{
			if (mainnull)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
					return;
				}
				Pick(Npcid, menu: false, getTask: true);
				Lock(1000);
				return;
			}
			if (index == 0)
			{
				if (TileMap.mapID != 28)
				{
					Go(28);
				}
				else
				{
					Attack(-1, -1);
				}
				return;
			}
			if (index == 1)
			{
				if (TileMap.mapID != 27)
				{
					Go(27);
					Sleep();
				}
				int num = 0;
				Service.gI().requestFriend();
				Sleep();
				for (int j = 0; j < GameScr.vCharInMap.size(); j++)
				{
					Char @char = (Char)GameScr.vCharInMap.elementAt(j);
					if (GameScr.vFriend.size() == 0)
					{
						Service.gI().addFriend(@char.cName);
					}
					else if (GameScr.gI().checkExistFriend(@char.cName))
					{
						num++;
						if (num == 5)
						{
							break;
						}
						Service.gI().addFriend(@char.cName);
						Sleep(500);
					}
				}
				if (TileMap.mapID != 1)
				{
					Go(1);
					Sleep();
				}
				num = 0;
				Service.gI().requestFriend();
				Sleep();
				for (int k = 0; k < GameScr.vCharInMap.size(); k++)
				{
					Char char2 = (Char)GameScr.vCharInMap.elementAt(k);
					if (GameScr.vFriend.size() == 0)
					{
						Service.gI().addFriend(char2.cName);
					}
					else if (GameScr.gI().checkExistFriend(char2.cName))
					{
						num++;
						if (num == 5)
						{
							break;
						}
						Service.gI().addFriend(char2.cName);
						Sleep(500);
					}
				}
				return;
			}
			if (index == 2)
			{
				if (TileMap.mapID != mapId)
				{
					Go(mapId);
					return;
				}
				Pick(Npcid, menu: true);
				Lock(1000);
				return;
			}
		}
		if (taskId != 12)
		{
			return;
		}
		if (mainnull)
		{
			if (TileMap.mapID != mapId)
			{
				Go(mapId);
				return;
			}
			Pick(Npcid, menu: false, getTask: true);
			Lock(1000);
		}
		else if (index == 0)
		{
			if (TileMap.mapID != 28)
			{
				Go(28);
			}
			else
			{
				Attack(-1, -1);
			}
		}
		else
		{
			if (index != 1)
			{
				return;
			}
			if (CheckPercentVuKhi())
			{
				if (TileMap.mapID != 22)
				{
					Go(22);
				}
				else
				{
					if (Char.FindNpc(6) == null)
					{
						return;
					}
					if (!GameScr.isPaintUpGrade)
					{
						Pick(6, menu: true);
					}
					if (ItemBodyItemID(Idvukhi()) != null)
					{
						Service.gI().itemBodyToBag(ItemBodyItemID(Idvukhi()).indexUI);
						Sleep(500);
						return;
					}
					if (ItemBagItemId(Idvukhi()) != null)
					{
						GameScr.itemUpGrade = ItemBagItemId(Idvukhi());
						Char.getMyChar().arrItemBag[GameScr.itemUpGrade.indexUI] = null;
						Sleep(500);
						return;
					}
					for (int l = 0; l < GameScr.arrItemUpGrade.Length; l++)
					{
						for (int m = 0; m < Char.getMyChar().arrItemBag.Length; m++)
						{
							if (Char.getMyChar().arrItemBag[m] != null && Char.getMyChar().arrItemBag[m].template.type == 26 && GameScr.arrItemUpGrade[l] == null)
							{
								GameScr.arrItemUpGrade[l] = Char.getMyChar().arrItemBag[m];
								Char.getMyChar().arrItemBag[m] = null;
								return;
							}
						}
					}
					if (GameScr.itemUpGrade != null)
					{
						Service.gI().upgradeItem(GameScr.itemUpGrade, GameScr.arrItemUpGrade, GameScr.isPaintUpGradeGold);
					}
				}
			}
			else if (TileMap.mapID != 46)
			{
				Go(46);
			}
			else
			{
				PickDa();
				Attack(-1, -1);
			}
		}
	}

	public static void Lock(int milisSeconds = 2000)
	{
		isTask = true;
		lock (objTask)
		{
			Monitor.Wait(objTask, milisSeconds);
		}
		Sleep(200);
	}

	public static void StopLock()
	{
		if (isTask)
		{
			lock (objTask)
			{
				Monitor.PulseAll(objTask);
			}
			isTask = false;
		}
	}

	static AutoTask()
	{
		objTask = new object();
		objSleep = new object();
	}

	public void Pick(int npcId, bool menu = false, bool getTask = false, int between = 0, int last = 0)
	{
		Npc npc = Char.FindNpc(npcId);
		if (npc != null)
		{
			Char.Move(npc.cx, npc.cy);
			if (menu)
			{
				Service.gI().menu(npcId, between, last);
			}
			if (getTask)
			{
				Service.gI().getTask(npcId, between, -1);
			}
		}
	}

	public Item ItemBagItemId(int id)
	{
		for (int i = 0; i < Char.getMyChar().arrItemBag.Length; i++)
		{
			Item item = Char.getMyChar().arrItemBag[i];
			if (item != null && item.template.id == id)
			{
				return item;
			}
		}
		return null;
	}

	public Item ItemBoxItemId(int id)
	{
		for (int i = 0; i < Char.getMyChar().arrItemBox.Length; i++)
		{
			Item item = Char.getMyChar().arrItemBox[i];
			if (item != null && item.template.id == id)
			{
				return item;
			}
		}
		return null;
	}

	public Item ItemBodyItemID(int id)
	{
		for (int i = 0; i < Char.getMyChar().arrItemBody.Length; i++)
		{
			Item item = Char.getMyChar().arrItemBody[i];
			if (item != null && item.template.id == id)
			{
				return item;
			}
		}
		return null;
	}

	public void Fix()
	{
		if (TileMap.mapID != 22)
		{
			Go(22);
		}
		else if (ItemBodyItemID(194) == null)
		{
			if (ItemBagItemId(194) != null)
			{
				Service.gI().useItem(ItemBagItemId(194).indexUI);
				Sleep();
			}
			else
			{
				Pick(5);
				Service.gI().itemBoxToBag(ItemBoxItemId(194).indexUI);
				Sleep();
			}
		}
	}

	public void S(int w)
	{
		Pick(4, menu: true);
		Sleep();
		if (!GameScr.isPaintGroceryLock)
		{
			return;
		}
		for (int i = 0; i < GameScr.arrItemGroceryLock.Length; i++)
		{
			Item item = GameScr.arrItemGroceryLock[i];
			if (item != null && item.template.id == 23)
			{
				Service.gI().buyItem(item.typeUI, item.indexUI, w);
				Lock(1000);
				break;
			}
		}
	}

	public int ThucAn()
	{
		int clevel = Char.getMyChar().clevel;
		if (clevel < 10)
		{
			return 1;
		}
		if (clevel >= 10 && clevel < 20)
		{
			return 10;
		}
		if (clevel >= 20 && clevel < 30)
		{
			return 20;
		}
		if (clevel >= 30 && clevel < 40)
		{
			return 30;
		}
		if (clevel >= 40 && clevel < 50)
		{
			return 40;
		}
		if (clevel >= 50 && clevel < 130)
		{
			return 50;
		}
		return 1;
	}

	public void Vo()
	{
		if (ItemBodyItemID(194) != null)
		{
			Service.gI().itemBodyToBag(ItemBodyItemID(194).indexUI);
			Sleep();
		}
		else if (Phai.Equals("kiem"))
		{
			if (TileMap.mapID != 1)
			{
				Go(1);
				return;
			}
			Pick(9, menu: true, getTask: false, 1);
			Lock(5000);
		}
		else if (Phai.Equals("tieu"))
		{
			if (TileMap.mapID != 1)
			{
				Go(1);
				return;
			}
			Pick(9, menu: true, getTask: false, 1, 1);
			Lock(5000);
		}
		else if (Phai.Equals("dao"))
		{
			if (TileMap.mapID != 27)
			{
				Go(27);
				return;
			}
			Pick(11, menu: true, getTask: false, 1);
			Lock(5000);
		}
		else if (Phai.Equals("quat"))
		{
			if (TileMap.mapID != 27)
			{
				Go(27);
				return;
			}
			Pick(11, menu: true, getTask: false, 1, 1);
			Lock(5000);
		}
		else if (Phai.Equals("kunai"))
		{
			if (TileMap.mapID != 72)
			{
				Go(72);
				return;
			}
			Pick(10, menu: true, getTask: false, 1);
			Lock(5000);
		}
		else if (Phai.Equals("cung"))
		{
			if (TileMap.mapID != 72)
			{
				Go(72);
				return;
			}
			Pick(10, menu: true, getTask: false, 1, 1);
			Lock(5000);
		}
		else if (TileMap.mapID != 27)
		{
			Go(27);
		}
		else
		{
			Pick(11, menu: true, getTask: false, 1, 1);
			Lock(5000);
		}
	}

	public void SVC()
	{
		Item item = ItemBagItemId(idSVC());
		if (item != null)
		{
			Service.gI().useItem(item.indexUI);
			Lock(1000);
		}
		else if (!Char.getMyChar().isLang())
		{
			Code.Die();
		}
		else if (Char.FindNpc(5) != null)
		{
			Pick(5);
			if (me.arrItemBox == null)
			{
				UI(4);
			}
			if (ItemBoxItemId(idSVC()) != null)
			{
				Service.gI().itemBoxToBag(ItemBoxItemId(idSVC()).indexUI);
				Sleep(400);
			}
		}
	}

	public void LuuToaDo()
	{
		if (TileMap.mapID != 27)
		{
			Go(27);
			Sleep();
		}
		if (TileMap.mapID != 27)
		{
			TileMap.LockMap(2000);
		}
		Pick(5, menu: true, getTask: false, 1);
		Sleep();
		saveTD = 27;
	}

	public int idThucAn()
	{
		if (ThucAn() == 1)
		{
			return 23;
		}
		if (ThucAn() == 10)
		{
			return 24;
		}
		if (ThucAn() == 20)
		{
			return 25;
		}
		if (ThucAn() == 30)
		{
			return 26;
		}
		if (ThucAn() == 40)
		{
			return 27;
		}
		if (ThucAn() == 50)
		{
			return 29;
		}
		return 23;
	}

	public void Mua(int id, int sl)
	{
		Pick(4);
		if (GameScr.arrItemGroceryLock == null)
		{
			UI(9);
			return;
		}
		for (int i = 0; i < GameScr.arrItemGroceryLock.Length; i++)
		{
			Item item = GameScr.arrItemGroceryLock[i];
			if (item != null && item.template.id == id)
			{
				Service.gI().buyItem(item.typeUI, item.indexUI, sl);
				if (taskId == 3 && !mainnull && index == 0)
				{
					Lock(4000);
				}
				else
				{
					CLock.LockBuy();
				}
				break;
			}
		}
	}

	public void VuKhi()
	{
		if (Char.getMyChar().nClass.classId == 4 && ItemBodyItemID(Idvukhi()) == null)
		{
			Item item = ItemBagItemId(Idvukhi());
			if (item != null)
			{
				Service.gI().useItem(item.indexUI);
				Sleep(200);
			}
			else if (!Char.getMyChar().isLang())
			{
				Code.Die();
			}
			else if (Char.FindNpc(5) != null)
			{
				Pick(5);
				if (me.arrItemBox == null)
				{
					UI(4);
				}
				if (ItemBoxItemId(Idvukhi()) != null)
				{
					Service.gI().itemBoxToBag(ItemBoxItemId(Idvukhi()).indexUI);
					Sleep();
				}
			}
		}
		else if (Char.getMyChar().nClass.classId == 1 && ItemBodyItemID(Idvukhi()) == null)
		{
			Item item2 = ItemBagItemId(Idvukhi());
			if (item2 != null)
			{
				Service.gI().useItem(item2.indexUI);
				Sleep(200);
			}
			else if (!Char.getMyChar().isLang())
			{
				Code.Die();
			}
			else if (Char.FindNpc(5) != null)
			{
				Pick(5);
				if (me.arrItemBox == null)
				{
					UI(4);
				}
				if (ItemBoxItemId(Idvukhi()) != null)
				{
					Service.gI().itemBoxToBag(ItemBoxItemId(Idvukhi()).indexUI);
					Sleep();
				}
			}
		}
		else if (Char.getMyChar().nClass.classId == 2 && ItemBodyItemID(Idvukhi()) == null)
		{
			Item item3 = ItemBagItemId(Idvukhi());
			if (item3 != null)
			{
				Service.gI().useItem(item3.indexUI);
				Sleep(200);
			}
			else if (!Char.getMyChar().isLang())
			{
				Code.Die();
			}
			else if (Char.FindNpc(5) != null)
			{
				Pick(5);
				if (me.arrItemBox == null)
				{
					UI(4);
				}
				if (ItemBoxItemId(Idvukhi()) != null)
				{
					Service.gI().itemBoxToBag(ItemBoxItemId(Idvukhi()).indexUI);
					Sleep();
				}
			}
		}
		else if (Char.getMyChar().nClass.classId == 6 && ItemBodyItemID(Idvukhi()) == null)
		{
			Item item4 = ItemBagItemId(Idvukhi());
			if (item4 != null)
			{
				Service.gI().useItem(item4.indexUI);
				Sleep(200);
			}
			else if (!Char.getMyChar().isLang())
			{
				Code.Die();
			}
			else if (Char.FindNpc(5) != null)
			{
				Pick(5);
				if (me.arrItemBox == null)
				{
					UI(4);
				}
				if (ItemBoxItemId(Idvukhi()) != null)
				{
					Service.gI().itemBoxToBag(ItemBoxItemId(Idvukhi()).indexUI);
					Sleep();
				}
			}
		}
		else if (Char.getMyChar().nClass.classId == 3 && ItemBodyItemID(Idvukhi()) == null)
		{
			Item item5 = ItemBagItemId(Idvukhi());
			if (item5 != null)
			{
				Service.gI().useItem(item5.indexUI);
				Sleep(200);
			}
			else if (!Char.getMyChar().isLang())
			{
				Code.Die();
			}
			else if (Char.FindNpc(5) != null)
			{
				Pick(5);
				if (me.arrItemBox == null)
				{
					UI(4);
				}
				if (ItemBoxItemId(Idvukhi()) != null)
				{
					Service.gI().itemBoxToBag(ItemBoxItemId(Idvukhi()).indexUI);
					Sleep();
				}
			}
		}
		else
		{
			if (Char.getMyChar().nClass.classId != 5 || ItemBodyItemID(119) != null)
			{
				return;
			}
			Item item6 = ItemBagItemId(119);
			if (item6 != null)
			{
				Service.gI().useItem(item6.indexUI);
				Sleep(200);
			}
			else if (!Char.getMyChar().isLang())
			{
				Code.Die();
			}
			else if (Char.FindNpc(5) != null)
			{
				Pick(5);
				if (me.arrItemBox == null)
				{
					UI(4);
				}
				if (ItemBoxItemId(119) != null)
				{
					Service.gI().itemBoxToBag(ItemBoxItemId(119).indexUI);
					Sleep();
				}
			}
		}
	}

	public bool isTruong()
	{
		if (TileMap.mapID != 1 && TileMap.mapID != 72)
		{
			return TileMap.mapID == 27;
		}
		return true;
	}

	public void UseKiemGo()
	{
		if (ItemBodyItemID(194) != null)
		{
			return;
		}
		if (ItemBagItemId(194) != null)
		{
			Service.gI().useItem(ItemBagItemId(194).indexUI);
			Sleep();
			return;
		}
		if (!Char.getMyChar().isLang())
		{
			Code.Die();
			Sleep(500);
		}
		if (Char.FindNpc(5) != null)
		{
			Pick(5);
			if (me.arrItemBox == null)
			{
				UI(4);
			}
			if (ItemBoxItemId(194) != null)
			{
				Service.gI().itemBoxToBag(ItemBoxItemId(194).indexUI);
				Sleep(400);
			}
		}
	}

	public void upPotential()
	{
		if (mSystem.currentTimeMillis() - timeuptn < 1000)
		{
			return;
		}
		if (Char.getMyChar().nClass.classId == 1)
		{
			if (Char.getMyChar().clevel <= 10)
			{
				if (Char.getMyChar().pPoint > 0)
				{
					if (Char.getMyChar().potential[2] < 35)
					{
						Service.gI().upPotential(2, (Char.getMyChar().pPoint >= 35 - Char.getMyChar().potential[2]) ? (35 - Char.getMyChar().potential[2]) : Char.getMyChar().pPoint);
						Sleep(100);
					}
					else
					{
						Service.gI().upPotential(0, Char.getMyChar().pPoint);
						Sleep(100);
					}
				}
			}
			else if (Char.getMyChar().clevel >= 11 && Char.getMyChar().pPoint > 0 && (taskId != 9 || mainnull || index != 1))
			{
				if (Char.getMyChar().potential[2] < 50)
				{
					Service.gI().upPotential(2, (Char.getMyChar().pPoint >= 50 - Char.getMyChar().potential[2]) ? (50 - Char.getMyChar().potential[2]) : Char.getMyChar().pPoint);
					Sleep(100);
				}
				else
				{
					Service.gI().upPotential(0, Char.getMyChar().pPoint);
					Sleep(100);
				}
			}
		}
		else if (Char.getMyChar().nClass.classId == 6)
		{
			if (Char.getMyChar().clevel <= 10)
			{
				if (Char.getMyChar().pPoint > 0)
				{
					if (Char.getMyChar().potential[2] < 35)
					{
						Service.gI().upPotential(2, (Char.getMyChar().pPoint >= 35 - Char.getMyChar().potential[2]) ? (35 - Char.getMyChar().potential[2]) : Char.getMyChar().pPoint);
						Sleep(100);
					}
					else
					{
						Service.gI().upPotential(3, Char.getMyChar().pPoint);
						Sleep(100);
					}
				}
			}
			else if (Char.getMyChar().clevel >= 11 && Char.getMyChar().pPoint > 0 && (taskId != 9 || mainnull || index != 1))
			{
				if (Char.getMyChar().potential[2] < 50)
				{
					Service.gI().upPotential(2, (Char.getMyChar().pPoint >= 50 - Char.getMyChar().potential[2]) ? (50 - Char.getMyChar().potential[2]) : Char.getMyChar().pPoint);
					Sleep(100);
				}
				else
				{
					Service.gI().upPotential(3, Char.getMyChar().pPoint);
					Sleep(100);
				}
			}
		}
		else
		{
			if (Char.getMyChar().nClass.classId != 3)
			{
				return;
			}
			if (Char.getMyChar().clevel <= 10)
			{
				if (Char.getMyChar().pPoint > 0)
				{
					if (Char.getMyChar().potential[2] < 35)
					{
						Service.gI().upPotential(2, (Char.getMyChar().pPoint >= 35 - Char.getMyChar().potential[2]) ? (35 - Char.getMyChar().potential[2]) : Char.getMyChar().pPoint);
						Sleep(100);
					}
					else
					{
						Service.gI().upPotential(0, Char.getMyChar().pPoint);
						Sleep(100);
					}
				}
			}
			else if (Char.getMyChar().clevel >= 11 && Char.getMyChar().pPoint > 0 && (taskId != 9 || mainnull || index != 1))
			{
				if (Char.getMyChar().potential[2] < 50)
				{
					Service.gI().upPotential(2, (Char.getMyChar().pPoint >= 50 - Char.getMyChar().potential[2]) ? (50 - Char.getMyChar().potential[2]) : Char.getMyChar().pPoint);
					Sleep(100);
				}
				else
				{
					Service.gI().upPotential(0, Char.getMyChar().pPoint);
					Sleep(100);
				}
			}
		}
	}

	public void TuDong()
	{
		bool flag = false;
		for (int i = 0; i < Char.getMyChar().vEff.size(); i++)
		{
			Effect effect = (Effect)Char.getMyChar().vEff.elementAt(i);
			if (effect != null && effect.template.id == EffidThucAn())
			{
				flag = true;
			}
		}
		if (!flag && ItemBagItemId(idThucAn()) != null)
		{
			Service.gI().useItem(ItemBagItemId(idThucAn()).indexUI);
		}
		if (mSystem.currentTimeMillis() - bagsort >= 10000)
		{
			Service.gI().bagSort();
			bagsort = mSystem.currentTimeMillis();
		}
		if (mSystem.currentTimeMillis() - boxsort >= 10000 && me.arrItemBox != null)
		{
			Service.gI().boxSort();
			boxsort = mSystem.currentTimeMillis();
		}
		if ((taskId > 2 && taskId < 8) || (taskId == 2 && !mainnull && index > 0) || (taskId == 8 && !mainnull && index <= 1))
		{
			UseKiemGo();
		}
		if (taskId >= 9 && !GameScr.isPaintUpGrade)
		{
			VuKhi();
		}
		Char.isAFood = true;
		Char.aFoodValue = ThucAn();
		upPotential();
	}

	public int EffidThucAn()
	{
		if (ThucAn() == 1)
		{
			return 0;
		}
		if (ThucAn() == 10)
		{
			return 1;
		}
		if (ThucAn() == 20)
		{
			return 2;
		}
		if (ThucAn() == 30)
		{
			return 3;
		}
		if (ThucAn() == 40)
		{
			return 4;
		}
		if (ThucAn() == 50)
		{
			return 28;
		}
		return 0;
	}

	public void upSkillTask()
	{
		if (Char.getMyChar().sPoint > 0)
		{
			if (Char.getMyChar().nClass.classId == 6)
			{
				Service.gI().upSkill(46, 1);
				Lock(1000);
			}
			else if (Char.getMyChar().nClass.classId == 3)
			{
				Service.gI().upSkill(19, 1);
				Lock(1000);
			}
		}
	}

	public void a()
	{
		if (taskId > 2 && ItemBagItemId(idThucAn()) == null && (taskId != 3 || !mainnull))
		{
			if (!Char.getMyChar().isLang())
			{
				Code.Die();
				return;
			}
			Mua(idThucAn(), 2);
			Sleep();
		}
		Default();
		Start();
	}

	public static void Sleep(int milisSeconds = 1000)
	{
		lock (objSleep)
		{
			Monitor.Wait(objSleep, milisSeconds);
		}
	}

	public bool CheckPercentVuKhi()
	{
		int num = 0;
		for (int i = 0; i < Char.getMyChar().arrItemBag.Length; i++)
		{
			Item item = Char.getMyChar().arrItemBag[i];
			if (item != null && item.template.type == 26)
			{
				num += item.getPecentUpgradeVuKhi(0);
				if (num >= 80)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool CheckPercentClothes()
	{
		int num = 0;
		for (int i = 0; i < Char.getMyChar().arrItemBag.Length; i++)
		{
			Item item = Char.getMyChar().arrItemBag[i];
			if (item != null && item.template.type == 26)
			{
				num += item.getPecentUpgradeDoLeft(0);
				if (num >= 80)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void UI(int typeUI)
	{
		Service.gI().requestItem(typeUI);
		CLock.LockTypeUI(typeUI, 5000);
	}

	public int Idvukhi()
	{
		if (me.nClass.classId == 6)
		{
			return 119;
		}
		if (me.nClass.classId == 4)
		{
			return 109;
		}
		if (me.nClass.classId == 1)
		{
			return 94;
		}
		if (me.nClass.classId == 2)
		{
			return 65;
		}
		if (me.nClass.classId == 3)
		{
			return 99;
		}
		return 194;
	}

	public int idSVC()
	{
		if (Char.getMyChar().nClass.classId == 4)
		{
			return 67;
		}
		if (Char.getMyChar().nClass.classId == 1)
		{
			return 40;
		}
		if (Char.getMyChar().nClass.classId == 6)
		{
			return 85;
		}
		if (Char.getMyChar().nClass.classId == 3)
		{
			return 58;
		}
		return 0;
	}
}
