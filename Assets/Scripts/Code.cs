using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using UnityEngine;

public class Code
{
	public static int dem;

	public static int count;

	public static Thread GetThread;

	public static ThreadStart GetthreadStart;

	public static Auto T;

	public static bool DkChay;

	public static TanSat tanSat;

	public static long w;

	public static MyVector s;

	public static long timeChangeZone;

	public static int demTask;

	public static AutoTask autoTask;

	public static Buff buff;

	public static int ccc;

	public static int CharIDSave;

	public static bool isHackGiay;

	public static int HackGiay;

	public static MyVector vCharInMap;

	public static MyVector vItemMap;

	public static bool FirstZone;

	public static bool bagshort;

	public static int test;

	public static MyVector Item;

	public static long timeDo;

	public static bool achat;

	public static int SpNextMap;

	public static int NextMinMaxInt => new System.Random().Next(int.MinValue, int.MaxValue);

	public static void Sleep(int milisSeconds)
	{
		try
		{
			Thread.Sleep(milisSeconds);
		}
		catch (ArgumentOutOfRangeException)
		{
		}
	}

	public static bool ChatMod(string text)
	{
		int num = 0;
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = new StringBuilder();
		for (int i = 0; i < text.Length; i++)
		{
			char c;
			if (((c = text[i]) >= '0' && c <= '9') || c == ' ')
			{
				for (; i < text.Length; i++)
				{
					char c2;
					if ((c2 = text[i]) < '0')
					{
						break;
					}
					if (c2 > '9')
					{
						break;
					}
					stringBuilder2.Append(c2);
				}
				break;
			}
			stringBuilder.Append(c);
		}
		text = stringBuilder.ToString().ToLower();
		if (stringBuilder2.Length > 0)
		{
			try
			{
				num = int.Parse(stringBuilder2.ToString());
			}
			catch (Exception)
			{
			}
		}
		if (text.Equals("u"))
		{
			if (num == 0)
			{
				num = 50;
			}
			Paint("Khinh kông " + num);
			Char.Move(Char.getMyChar().cx, Char.getMyChar().cy - num);
			return true;
		}
		if (text.Equals("d"))
		{
			if (num == 0)
			{
				num = 50;
			}
			Paint("Độn thổ " + num);
			Char.Move(Char.getMyChar().cx, Char.getMyChar().cy + num);
			return true;
		}
		if (text.Equals("r"))
		{
			if (num == 0)
			{
				num = 50;
			}
			Paint("Dịch phải " + num);
			Char.Move(Char.getMyChar().cx + num, Char.getMyChar().cy);
			return true;
		}
		if (text.Equals("l"))
		{
			if (num == 0)
			{
				num = 50;
			}
			Paint("Dịch trái " + num);
			Char.Move(Char.getMyChar().cx - num, Char.getMyChar().cy);
			return true;
		}
		if (text.Equals("g"))
		{
			new Thread((ThreadStart)delegate
			{
				Char myChar = Char.getMyChar();
				if (myChar.charFocus != null)
				{
					Paint("MoveTo " + myChar.charFocus.cName);
					Char.Move(myChar.charFocus.cx, myChar.charFocus.cy);
				}
				else if (myChar.npcFocus != null)
				{
					Paint("MoveTo " + myChar.npcFocus.template.name);
					Char.Move(myChar.npcFocus.cx, myChar.npcFocus.cy);
				}
				else if (myChar.mobFocus != null)
				{
					Paint("MoveTo " + myChar.mobFocus.getTemplate().name);
					Char.Move(myChar.mobFocus.xFirst, myChar.mobFocus.yFirst);
				}
				else if (myChar.itemFocus != null)
				{
					Paint("MoveTo " + myChar.itemFocus.template.name);
					Char.Move(myChar.itemFocus.x, myChar.itemFocus.y);
				}
			}).Start();
			return true;
		}
		if (text.Contains("npc"))
		{
			Paint("Act NPC: " + num);
			Npc npc = Char.FindNpc(num);
			if (Math.abs(Char.getMyChar().cx - npc.cx) > 22 || Math.abs(Char.getMyChar().cy - npc.cy) > 22)
			{
				Char.Move(npc.cx, npc.cy);
			}
			return true;
		}
		if (text.Equals("gm"))
		{
			Paint("Đến map: " + TileMap.mapNames[num]);
			GoMap(num);
			return true;
		}
		if (text.Equals("nm"))
		{
			Paint("Next map: " + num);
			NextMap(num);
			return true;
		}
		if (text.Equals("hs"))
		{
			Paint("Next to Hirosaki");
			GoMap(1);
			return true;
		}
		if (text.Equals("hr"))
		{
			Paint("Next to Harunna");
			GoMap(27);
			return true;
		}
		if (text.Equals("oz"))
		{
			Paint("Next to Oozaka");
			GoMap(72);
			return true;
		}
		if (text.Equals("kj"))
		{
			Paint("Next to Kojin");
			GoMap(10);
			return true;
		}
		if (text.Equals("sz"))
		{
			Paint("Next to Sanzu");
			GoMap(17);
			return true;
		}
		if (text.Equals("tn"))
		{
			Paint("Next to Tone");
			GoMap(22);
			return true;
		}
		if (text.Equals("lc"))
		{
			Paint("Next to Chài");
			GoMap(32);
			return true;
		}
		if (text.Equals("ck"))
		{
			Paint("Next to Chakumi");
			GoMap(38);
			return true;
		}
		if (text.Equals("eg"))
		{
			Paint("Next to Echigo");
			GoMap(43);
			return true;
		}
		if (text.Equals("os"))
		{
			Paint("Next to Oshin");
			GoMap(48);
			return true;
		}
		if (text.Equals("ts"))
		{
			if (Mob.MobContain(num))
			{
				Paint("Tàn sát: " + Mob.MobName(num));
				TS(num);
			}
			else
			{
				Paint("Tàn sát: all");
				TS(-1);
			}
			return true;
		}
		if (text.Equals("die"))
		{
			Paint("Tự sát");
			Die();
			return true;
		}
		if (text.Equals("e") || text.Equals("pe"))
		{
			Paint("End Auto");
			Abort();
			return true;
		}
		if (text.Equals("atkiem"))
		{
			if (num == 0)
			{
				num = -1;
			}
			Paint("Auto nhiệm vụ");
			AT("kiem", num);
			return true;
		}
		if (text.Equals("attieu"))
		{
			if (num == 0)
			{
				num = -1;
			}
			Paint("Auto nhiệm vụ");
			AT("tieu", num);
			return true;
		}
		if (text.Equals("atdao"))
		{
			if (num == 0)
			{
				num = -1;
			}
			Paint("Auto nhiệm vụ");
			AT("dao", num);
			return true;
		}
		if (text.Equals("atkunai"))
		{
			if (num == 0)
			{
				num = -1;
			}
			Paint("Auto nhiệm vụ");
			AT("kunai", num);
			return true;
		}
		if (text.Equals("atquat"))
		{
			if (num == 0)
			{
				num = -1;
			}
			Paint("Auto nhiệm vụ");
			AT("quat", num);
			return true;
		}
		if (text.Equals("atcung"))
		{
			if (num == 0)
			{
				num = -1;
			}
			Paint("Auto nhiệm vụ");
			AT("cung", num);
			return true;
		}
		if (text.Equals("bux"))
		{
			if (GameScr.vParty.size() > 0 && Char.getMyChar().nClass.classId == 6 && Char.getMyChar() != ((Party)GameScr.vParty.firstElement()).c)
			{
				Paint("chỉ buff xa");
				BHSX(isHsx: false);
			}
			return true;
		}
		if (text.Equals("hsx"))
		{
			if (GameScr.vParty.size() > 0 && Char.getMyChar().nClass.classId == 6 && Char.getMyChar() != ((Party)GameScr.vParty.firstElement()).c)
			{
				Paint("chỉ hsx xa");
				BHSX(isHsx: true, isBuff: false);
			}
			return true;
		}
		if (text.Equals("buff"))
		{
			if (GameScr.vParty.size() > 0 && Char.getMyChar().nClass.classId == 6 && Char.getMyChar() != ((Party)GameScr.vParty.firstElement()).c)
			{
				Paint("Buff hsx team");
				BHSX();
			}
			return true;
		}
		if (text.Equals("mnv"))
		{
			Paint("Đến map nhiệm vụ");
			GoMap(GameScr.gI().getTaskMapId());
			return true;
		}
		if (text.Equals("mnvp"))
		{
			Paint("Đến map nhiệm vụ hằng ngày");
			GoMap(Char.FindTask(0).mapId);
			return true;
		}
		if (text.Equals("mnvtt"))
		{
			Paint("Đến map nhiệm vụ tà thú");
			GoMap(Char.FindTask(1).mapId);
			return true;
		}
		if (text.Equals("s"))
		{
			Paint("Fake tốc độ chạy: " + num);
			isHackGiay = true;
			HackGiay = num;
			return true;
		}
		if (text.Equals("rs"))
		{
			Paint("Reset tốc chạy");
			isHackGiay = false;
			return true;
		}
		if (text.Contains("nv"))
		{
			GameScr.nhanvat = !GameScr.nhanvat;
			Paint("Hiển thị nhận vật trong map: " + (GameScr.nhanvat ? "Bật" : "Tắt"));
			return true;
		}
		if (text.Contains("ltd"))
		{
			autoTask.Pick(5, menu: true, getTask: false, 1);
			return true;
		}
		if (text.Contains("ak"))
		{
			GameScr.isTudanh = !GameScr.isTudanh;
			Paint("Tự Đánh: " + (GameScr.isTudanh ? "Bật" : "Tắt"));
			return true;
		}
		if (text.Contains("glv"))
		{
			if (GameScr.GiuLV = !GameScr.GiuLV)
			{
				Paint("Bật giữ level");
			}
			else
			{
				Paint("Tắt giữ level");
			}
			return true;
		}
		if (text.Contains("atc"))
		{
			achat = !achat;
			new Thread(Autochat).Start();
			Paint("Auto chat: " + (achat ? "Bật" : "Tắt"));
			return true;
		}
		if (text.Equals("tdm"))
		{
			if (num <= 0)
			{
				Paint("Nên đặt tốc độ NextMap từ 1 đến 5000ms");
			}
			else
			{
				Paint("Tốc độ NextMap: " + num + "ms");
				SpNextMap = num;
			}
			return true;
		}
		if (text.Contains("hsl"))
		{
			GameScr.HoiSinhLuong = !GameScr.HoiSinhLuong;
			Paint("HS lượng: " + (GameScr.HoiSinhLuong ? "Bật" : "Tắt"));
			return true;
		}
		if (text.Contains("skin"))
		{
			if (Char.getMyChar().charFocus != null)
			{
				Char.getMyChar().head = Char.getMyChar().charFocus.head;
				Char.getMyChar().body = Char.getMyChar().charFocus.body;
				Char.getMyChar().leg = Char.getMyChar().charFocus.leg;
				Char.getMyChar().cName = Char.getMyChar().charFocus.cName;
				Char.getMyChar().cClanName = Char.getMyChar().charFocus.cClanName;
				Char.getMyChar().ctypeClan = Char.getMyChar().charFocus.ctypeClan;
				Char.getMyChar().sType = Char.getMyChar().charFocus.sType;
				Char.getMyChar().vDomsang = Char.getMyChar().charFocus.vDomsang;
				Char.getMyChar().idpartThoitrang = Char.getMyChar().charFocus.idpartThoitrang;
			}
			if (Char.getMyChar().mobFocus != null)
			{
				Char.getMyChar().cp1 = Char.getMyChar().mobFocus.p1;
				Char.getMyChar().cp2 = Char.getMyChar().mobFocus.p2;
				Char.getMyChar().cp3 = Char.getMyChar().mobFocus.p3;
			}
			if (Char.getMyChar().npcFocus != null)
			{
				Char.getMyChar().head = Char.getMyChar().npcFocus.head;
				Char.getMyChar().body = Char.getMyChar().npcFocus.body;
				Char.getMyChar().cName = Char.getMyChar().npcFocus.cName;
			}
			Paint("Fake skin nhân vật");
			return true;
		}
		if (text.Contains("xdh"))
		{
			GameCanvas.lowGraphic = !GameCanvas.lowGraphic;
			Paint("Xóa địa hình: " + (GameCanvas.lowGraphic ? "Bật" : "Tắt"));
			return true;
		}
		if (text.Contains("k"))
		{
			Paint("Chuyển khu: " + num);
			Npc npc2 = Char.FindNpc(13);
			if (Math.abs(Char.getMyChar().cx - npc2.cx) > 22 || Math.abs(Char.getMyChar().cy - npc2.cy) > 22)
			{
				Char.Move(npc2.cx, npc2.cy);
			}
			Service.gI().requestChangeZone(num, -1);
			return true;
		}
		if (text.Contains("cnhat"))
		{
			GameScr.HutVatPham = !GameScr.HutVatPham;
			Paint(GameScr.HutVatPham ? "Bật hút VP" : "Bật nhặt xa");
			return true;
		}
		if (text.Contains("cmap"))
		{
			Auto.Actions.ChuyenMapHetBoss = !Auto.Actions.ChuyenMapHetBoss;
			Paint(Auto.Actions.ChuyenMapHetBoss ? "Bật: Chuyển map hết boss" : "Tắt: Chuyển map hết boss");
			return true;
		}
		return false;
	}

	public static void Paint(string text)
	{
		ChatPopup.addChatPopupMultiLine("[Ninja Hoa quả] " + text, 300, Char.getMyChar());
	}

	public static void GoMap(int mapID)
	{
		new Thread((ThreadStart)delegate
		{
			GameCanvas.startWaitDlgWithoutCancel();
			try
			{
				TileMap.GoMap(mapID);
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
			}
			GC.Collect();
			if (Session_ME.connecting)
			{
				GameScr.gI().switchToMe();
			}
			GameCanvas.endDlg();
			GameCanvas.isLoading = false;
		}).Start();
	}

	public static void GetPngFile(Image image, string savePath)
	{
		Texture2D texture = image.texture;
		texture.SetPixel(texture.width - 30, texture.height - 30, Color.black);
		texture.Apply();
		File.WriteAllBytes(savePath + ".png", image.texture.EncodeToPNG());
	}

	public static void Fixed()
	{
		dem = new System.Random().Next(0, 7);
		if (Auto.isDie(Char.getMyChar()) && (TileMap.IsDangGoMap || TileMap.MapMod != TileMap.mapID))
		{
			TileMap.HuyLockMap();
		}
		//StreamWriter streamWriter = new StreamWriter("NinjaMax/item.txt");
		//if (Char.getMyChar().itemFocus != null)
		//{
		//	FieldInfo[] fields = Char.getMyChar().itemFocus.GetType().GetFields();
		//	foreach (FieldInfo fieldInfo in fields)
		//	{
		//		streamWriter.WriteLine(fieldInfo.Name + ": " + fieldInfo.GetValue(Char.getMyChar().itemFocus));
		//	}
		//	streamWriter.WriteLine("template: ");
		//	fields = Char.getMyChar().itemFocus.template.GetType().GetFields();
		//	foreach (FieldInfo fieldInfo2 in fields)
		//	{
		//		streamWriter.WriteLine(fieldInfo2.Name + ": " + fieldInfo2.GetValue(Char.getMyChar().itemFocus.template));
		//	}
		//}
		//streamWriter.Close();
	}

	public static void GetImage()
	{
		try
		{
			while (true)
			{
				try
				{
					for (int i = 0; i < GameScr.vMob.size(); i++)
					{
						Mob mob = (Mob)GameScr.vMob.elementAt(i);
						if (mob == null)
						{
							continue;
						}
						MobTemplate mobTemplate = Mob.arrMobTemplate[mob.templateId];
						string path = string.Concat("Ảnh/x" + mGraphics.zoomLevel + "/map/", TileMap.mapID, "/mob/", mob.templateId);
						int num = dem;
						for (int j = 0; j < num; j++)
						{
							string text = string.Concat("Ảnh/x" + mGraphics.zoomLevel + "/map/", TileMap.mapID, "/mob/", mob.templateId, "/", j);
							Image image = mobTemplate.imgs[j];
							if (!Directory.Exists("Ảnh/x" + mGraphics.zoomLevel + "/map/" + TileMap.mapID))
							{
								Directory.CreateDirectory("Ảnh/x" + mGraphics.zoomLevel + "/map/" + TileMap.mapID);
							}
							if (!Directory.Exists("Ảnh/x" + mGraphics.zoomLevel + "/map/" + TileMap.mapID + "/mob"))
							{
								Directory.CreateDirectory("Ảnh/x" + mGraphics.zoomLevel + "/map/" + TileMap.mapID + "/mob");
							}
							if (!Directory.Exists(path))
							{
								Directory.CreateDirectory(path);
							}
							if (image != null && !File.Exists(text))
							{
								GetPngFile(image, text);
							}
						}
					}
				}
				catch (Exception)
				{
				}
				Sleep(10);
			}
		}
		catch (Exception)
		{
		}
	}

	static Code()
	{
		Item = new MyVector();
		SpNextMap = 500;
		test = 23123213;
		buff = new Buff();
		autoTask = new AutoTask();
		tanSat = new TanSat();
		s = new MyVector();
	}

	public static void NextMap(int way)
	{
		new Thread((ThreadStart)delegate
		{
			GameCanvas.startWaitDlgWithoutCancel();
			try
			{
				TileMap.NextMap(way);
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
			}
			GC.Collect();
			if (Session_ME.connecting)
			{
				GameScr.gI().switchToMe();
			}
			GameCanvas.endDlg();
			GameCanvas.isLoading = false;
		}).Start();
	}

	public static void Run()
	{
		try
		{
			while (DkChay)
			{
				long num = mSystem.currentTimeMillis();
				try
				{
					if (T != null)
					{
						T.Run();
					}
				}
				catch (Exception)
				{
				}
				long num2 = mSystem.currentTimeMillis() - num;
				if (mSystem.currentTimeMillis() - num < 100)
				{
					Thread.Sleep((int)(100 - num2));
				}
				else
				{
					Thread.Sleep(0);
				}
			}
		}
		catch (Exception)
		{
		}
	}

	public static void SwitchToMe()
	{
		if (!DkChay)
		{
			DkChay = true;
			GetthreadStart = Run;
			GetThread = new Thread(GetthreadStart);
			GetThread.Start();
		}
		CharIDSave = Char.getMyChar().charID;
		LoadAuto();
	}

	public static void Abort()
	{
		if (T != null)
		{
			T = null;
		}
		if (GetThread != null)
		{
			GetThread.Abort();
			GetThread = null;
			if (GetthreadStart != null)
			{
				GetthreadStart = null;
			}
		}
		GetthreadStart = Run;
		GetThread = new Thread(GetthreadStart);
		GetThread.Start();
	}

	public static void Stop()
	{
		Auto t = T;
		DkChay = false;
		if (T != null)
		{
			T = null;
		}
		if (GetThread != null)
		{
			GetThread.Abort();
			GetThread = null;
			if (GetthreadStart != null)
			{
				GetthreadStart = null;
			}
		}
		if (GetThread != null)
		{
			GetThread.Abort();
			GetThread = null;
			if (GetthreadStart != null)
			{
				GetthreadStart = null;
			}
		}
		T = t;
		SaveAuto(CharIDSave);
	}

	public static void S(Auto auto)
	{
		auto.T = T;
		T = auto;
	}

	public static bool boolItemBagId(int paramInt)
	{
		Item[] arrItemBag = Char.getMyChar().arrItemBag;
		for (int i = 0; i < arrItemBag.Length; i++)
		{
			if (arrItemBag[i] != null && arrItemBag[i].template.id == paramInt)
			{
				return true;
			}
		}
		return false;
	}

	public static void Die()
	{
		Npc npc = Char.FindNpc(13);
		if (TileMap.mapID == 20)
		{
			Char.Move(TileMap.pxw, TileMap.pxh);
		}
		else if (npc == null)
		{
			if (!boolItemBagId(37) && !boolItemBagId(35))
			{
				Char.Move(Char.getMyChar().cx, YDirt(Char.getMyChar().cx, Char.getMyChar().cy));
				Service.gI().openUIZone();
			}
			else
			{
				Char.Move(TileMap.pxw, TileMap.pxh);
			}
		}
		else if (Math.abs(npc.cx - Char.getMyChar().cx) <= 200 && Math.abs(npc.cy - Char.getMyChar().cy) <= 200)
		{
			if (!boolItemBagId(37) && !boolItemBagId(35))
			{
				if (npc.cx < Char.getMyChar().cx)
				{
					Char.Move((Char.getMyChar().cx + 200 >= TileMap.pxw) ? (Char.getMyChar().cx - 200 - Math.abs(Char.getMyChar().cx - npc.cx)) : (Char.getMyChar().cx + 200), YDirt(Char.getMyChar().cx + 200, Char.getMyChar().cy));
				}
				else
				{
					Char.Move((Char.getMyChar().cx - 200 <= 0) ? (Char.getMyChar().cx + 200 + Math.abs(Char.getMyChar().cx - npc.cx)) : (Char.getMyChar().cx - 200), YDirt(Char.getMyChar().cx + 200, Char.getMyChar().cy));
				}
				Char.Move(Char.getMyChar().cx, YDirt(Char.getMyChar().cx, Char.getMyChar().cy));
				Service.gI().openUIZone();
			}
			else
			{
				Char.Move(Char.getMyChar().cx, TileMap.pxh);
			}
		}
		else if (!boolItemBagId(37) && !boolItemBagId(35))
		{
			Char.Move(Char.getMyChar().cx, YDirt(Char.getMyChar().cx, Char.getMyChar().cy));
			Service.gI().openUIZone();
		}
		else
		{
			Char.Move(Char.getMyChar().cx, TileMap.pxh);
		}
	}

	public static int YDirt(int cx, int cy)
	{
		cy = TileMap.tileYofPixel(cy);
		for (int i = 0; i < 240; i++)
		{
			int num = cy + i * 12;
			if (TileMap.tileTypeAt(cx, num, 2))
			{
				return num;
			}
		}
		for (int j = 0; j < 240; j++)
		{
			int num2 = cy + j * 12;
			if (TileMap.tileTypeAt(cx, num2, 2))
			{
				return num2;
			}
		}
		return cy;
	}

	public static void PaintG(mGraphics g)
	{
	}

	public static void TS(int templateId, int mobId = -1)
	{
		tanSat.update(templateId, mobId);
		S(tanSat);
	}

	public static void AT(string phai, int LorETask = -1)
	{
		autoTask.update(phai, LorETask);
		S(autoTask);
	}

	public static void BHSX(bool isHsx = true, bool isBuff = true)
	{
		buff.update(isHsx, isBuff);
		S(buff);
	}

	public static bool ToBoolean(int value)
	{
		return value.Equals(0);
	}

	public static int ToInt32(bool value)
	{
		if (value.Equals(obj: true))
		{
			return 0;
		}
		return 1;
	}

	public static void LoadAuto()
	{
		SaveAuto(CharIDSave);
		Char.isAHP = RMS.loadRMSBool("-" + Char.getMyChar().charID + "-isAHP");
		Char.aHpValue = RMS.loadRMSInt("-" + Char.getMyChar().charID + "-aHpValue");
		Char.isAMP = RMS.loadRMSBool("-" + Char.getMyChar().charID + "-isAMP");
		Char.aMpValue = RMS.loadRMSInt("-" + Char.getMyChar().charID + "-aMpValue");
		Char.isAHP = RMS.loadRMSBool("-" + Char.getMyChar().charID + "-isAHP");
		Char.aHpValue = RMS.loadRMSInt("-" + Char.getMyChar().charID + "-aHpValue");
		Char.isAFood = RMS.loadRMSBool("-" + Char.getMyChar().charID + "-isAFood");
		Char.aFoodValue = RMS.loadRMSInt("-" + Char.getMyChar().charID + "-aFoodValue");
		Char.isABuff = RMS.loadRMSBool("-" + Char.getMyChar().charID + "-isABuff");
		bagshort = false;
	}

	public static void SaveAuto(int CharID)
	{
		timeChangeZone = 0L;
		RMS.SaveRMSBool("-" + CharID + "-isAHP", Char.isAHP);
		RMS.saveRMSInt("-" + CharID + "-aHpValue", Char.aHpValue);
		RMS.SaveRMSBool("-" + CharID + "-isAMP", Char.isAMP);
		RMS.saveRMSInt("-" + CharID + "-aMpValue", Char.aMpValue);
		RMS.SaveRMSBool("-" + CharID + "-isAFood", Char.isAFood);
		RMS.saveRMSInt("-" + CharID + "-aFoodValue", Char.aFoodValue);
		RMS.SaveRMSBool("-" + CharID + "-isABuff", Char.isABuff);
		string path = Application.persistentDataPath + "/buffmap";
		string path2 = Application.persistentDataPath + "/buffzone";
		File.WriteAllText(path, string.Concat(-2));
		File.WriteAllText(path2, string.Concat(-2));
	}

	public static void Autochat()
	{
		while (achat)
		{
			//string text = File.ReadAllText("hunghero.txt");
			//Service.gI().chat(text);
			//Thread.Sleep(5000);
		}
	}
}
