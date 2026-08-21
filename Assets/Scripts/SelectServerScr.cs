using System;
using System.IO;
using UnityEngine;

public class SelectServerScr : mScreen, IActionListener
{
	private int popupW;

	private int popupH;

	private int popupX;

	private int popupY;

	private static LanguageScr gI;

	private int indexRow = -1;

	public static bool isFromLogin;

	public static string[] menu;

	public static string uname;

	public static string pass;

	public static string unameChange;

	public static string passChange;

	public static Command cmdChoiMoi;

	public static Command cmdDoiTaiKhoan;

	public static Command cmdChoiTiep;

	public static Command cmdChonServer;

	public static Command cmdUpdLinkSv;

	public static Command[][] cmd;

	public static int ipSelect;

	public bool isFAQ;

	public string listFAQ = string.Empty;

	public string titleFAQ;

	public string subtitleFAQ;

	public string randomResuft;

	public static void loadIP()
	{
		ipSelect = loadIndexServer();
		if (ipSelect < 0 || GameMidlet.nameServer == null || ipSelect >= GameMidlet.nameServer.Length)
		{
			ipSelect = 0;
		}
		loadInfoIp(ipSelect);
	}

	public static void loadInfoIp(int index)
	{
		if (Session_ME.gI().isConnected())
		{
			Session_ME.gI().close();
		}
		if (GameMidlet.ipList == null || index < 0 || index >= GameMidlet.ipList.Length)
		{
			index = 0;
		}
		ipSelect = index;
		saveIndexServer(ipSelect);
		GameMidlet.IP = GameMidlet.ipList[ipSelect];
		GameMidlet.PORT = GameMidlet.portList[ipSelect];
		GameMidlet.serverLogin = GameMidlet.serverLoginList[ipSelect];
		mResources.loadLanguage(0);
		if (GameMidlet.serverST != null && ipSelect < GameMidlet.serverST.Length)
		{
			GameCanvas.menu.menuSelectedItem = GameMidlet.serverST[ipSelect];
		}
		GameCanvas.connect(5);
	}

	public override void switchToMe()
	{
		Code.Stop();
		if (RMS.loadRMSInt("isKiemduyet") == 0)
		{
			GameCanvas.isKiemduyet = true;
		}
		else
		{
			GameCanvas.isKiemduyet = false;
		}
		GameScr.gH = GameCanvas.h;
		if (GameCanvas.typeBg == 2)
		{
			GameCanvas.loadBG(0);
		}
		else
		{
			GameCanvas.loadBG(TileMap.bgID);
		}
		base.switchToMe();
		if (GameScr.instance != null)
		{
			GameScr.instance = null;
		}
		TileMap.bgID = (sbyte)(mSystem.currentTimeMillis() % 9);
		if (TileMap.bgID == 5 || TileMap.bgID == 6)
		{
			TileMap.bgID = 4;
		}
		GameScr.loadCamera(fullScreen: true);
		GameScr.cmx = 100;
		left = new Command(mResources.LANGUAGE, GameCanvas.instance, 8886, null);
		right = new Command("Tài khoản", this, 12345, null);
		indexRow = -1;
		if (!GameCanvas.isTouch)
		{
			indexRow = 0;
		}
		if (cmdChoiMoi == null)
		{
			cmdChoiMoi = new Command((!GameCanvas.isTouch) ? mResources.OK : string.Empty, this, 1000, null);
			cmdDoiTaiKhoan = new Command((!GameCanvas.isTouch) ? mResources.OK : string.Empty, this, 1001, null);
			cmdChonServer = new Command((!GameCanvas.isTouch) ? mResources.OK : string.Empty, this, 1002, null);
			cmdChoiTiep = new Command((!GameCanvas.isTouch) ? mResources.OK : string.Empty, this, 1003, null);
			cmd = new Command[2][]
			{
				new Command[3] { cmdChoiMoi, cmdDoiTaiKhoan, cmdChonServer },
				new Command[4] { cmdChoiTiep, cmdChoiMoi, cmdDoiTaiKhoan, cmdChonServer }
			};
		}
		uname = RMS.loadRMSString("acc");
		pass = RMS.loadRMSString("pass");
		if (uname == null)
		{
			uname = string.Empty;
		}
		if (pass == null)
		{
			pass = string.Empty;
		}
		if (string.IsNullOrEmpty(mResources.NEW_PLAY) || string.IsNullOrEmpty(mResources.SERVER))
		{
			mResources.loadLanguage(0);
		}
		if ((uname == null || uname.Equals(string.Empty)) && unameChange.Equals(string.Empty))
		{
			menu = new string[3]
			{
				mResources.NEW_PLAY,
				mResources.CHANGE_ACC,
				mResources.SERVER
			};
		}
		else
		{
			menu = new string[4]
			{
				mResources.COUNTINUE_PLAY,
				mResources.NEW_PLAY,
				mResources.CHANGE_ACC,
				mResources.SERVER
			};
		}
		popupW = 170;
		popupH = 50 + menu.Length * 35 + 10;
		if (GameCanvas.w == 128 || GameCanvas.h <= 208)
		{
			popupW = 126;
			popupH = 45 + menu.Length * 30 + 10;
		}
		popupX = GameCanvas.w / 2 - popupW / 2;
		popupY = GameCanvas.h / 2 - popupH / 2;
		if (GameCanvas.h <= 250)
		{
			popupY -= 10;
		}
		if (loadIndexServer() > -1 && GameMidlet.ipList != null && loadIndexServer() < GameMidlet.ipList.Length)
		{
			GameCanvas.menu.menuSelectedItem = loadIndexServer();
			GameMidlet.IP = GameMidlet.ipList[loadIndexServer()];
		}
		if (RMS.loadRMSString("random") == null)
		{
			RMS.saveRMSString("random", randomNumberlist());
		}
		if (LoginScr.imgTitle == null)
		{
			if (Main.isAppTeam)
			{
				LoginScr.imgTitle = GameCanvas.loadImage("/tt1");
			}
			else
			{
				LoginScr.imgTitle = GameCanvas.loadImage("/tt");
			}
		}
	}

	public override void paint(mGraphics g)
	{
		g.setColor(0);
		g.fillRect(0, 0, GameCanvas.w, GameCanvas.h);
		GameCanvas.paintBGGameScr(g);
		g.drawImage(LoginScr.imgTitle, GameCanvas.hw - LoginScr.imgTitle.getWidth() / 2, popupY + 10 - LoginScr.imgTitle.getHeight() / 2, 0);
		if (!GameCanvas.isTouch && GameCanvas.menu.menuSelectedItem == -1)
		{
			GameCanvas.menu.menuSelectedItem = 0;
		}
		int num = popupY + 50;
		int curInd = loadIndexServer();
		string serverName = (GameMidlet.nameServer != null && curInd >= 0 && curInd < GameMidlet.nameServer.Length) ? GameMidlet.nameServer[curInd] : string.Empty;
		for (int i = 0; i < menu.Length; i++)
		{
			bool isSelected = (i == indexRow);
			int btnX = popupX + 10;
			int btnY = num + i * 35;
			int btnW = popupW - 20;
			int btnH = 28;

			if (isSelected)
			{
				g.setColor(6505770);
				g.fillRect(btnX, btnY, btnW, btnH);
				GameCanvas.paintz.paintFrameBorder(btnX, btnY, btnW, btnH, g);
				GameCanvas.paintShukiren(btnX + 14, btnY + btnH / 2, g, noRotate: false);
				GameCanvas.paintShukiren(btnX + btnW - 14, btnY + btnH / 2, g, noRotate: false);
			}
			else
			{
				g.setColor(Paint.COLORDARK);
				g.fillRect(btnX, btnY, btnW, btnH);
				GameCanvas.paintz.paintFrameBorder(btnX, btnY, btnW, btnH, g);
			}

			mFont fontDraw = isSelected ? mFont.tahoma_7b_yellow : mFont.tahoma_7b_white;

			string itemText = menu[i];
			if (uname.Equals(string.Empty) && unameChange.Equals(string.Empty))
			{
				if (i == 2)
				{
					string prefix = (itemText.EndsWith(":") || itemText.EndsWith(": ")) ? itemText : (itemText + ": ");
					itemText = prefix + serverName;
				}
			}
			else
			{
				switch (i)
				{
				case 0:
					itemText = itemText + ((!unameChange.Equals(string.Empty)) ? (": " + unameChange) : ((!uname.StartsWith("tmpusr")) ? (": " + uname) : string.Empty));
					break;
				case 3:
					string prefix2 = (itemText.EndsWith(":") || itemText.EndsWith(": ")) ? itemText : (itemText + ": ");
					itemText = prefix2 + serverName;
					break;
				}
			}
			fontDraw.drawString(g, itemText, popupX + popupW / 2, btnY + (btnH - 16) / 2 + 1, 2);
		}
		if (GameCanvas.currentDialog == null)
		{
			GameCanvas.paintz.paintCmdBar(g, left, center, right);
		}
		base.paint(g);
	}

	public override void update()
	{
		if (uname.Equals(string.Empty) && unameChange.Equals(string.Empty))
		{
			if (indexRow > -1 && indexRow < cmd[0].Length)
			{
				center = cmd[0][indexRow];
			}
		}
		else if (indexRow > -1 && indexRow < cmd[1].Length)
		{
			center = cmd[1][indexRow];
		}
		GameScr.cmx++;
		if (GameScr.cmx > GameCanvas.w * 3 + 100)
		{
			GameScr.cmx = 100;
		}
		base.update();
	}

	public override void updateKey()
	{
		if (GameCanvas.keyPressedz[2] || GameCanvas.keyPressedz[4])
		{
			indexRow--;
			if (indexRow < 0)
			{
				indexRow = menu.Length - 1;
			}
		}
		else if (GameCanvas.keyPressedz[8] || GameCanvas.keyPressedz[6])
		{
			indexRow++;
			if (indexRow > menu.Length - 1)
			{
				indexRow = 0;
			}
		}
		if (GameCanvas.isPointerJustRelease && GameCanvas.isPointerHoldIn(popupX + 10, popupY + 45, popupW - 10, menu.Length * 35 + 10))
		{
			if (GameCanvas.isPointerClick)
			{
				indexRow = (GameCanvas.py - (popupY + 45)) / 35;
			}
			if (uname.Equals(string.Empty) && unameChange.Equals(string.Empty))
			{
				if (indexRow > -1 && indexRow < cmd[0].Length)
				{
					cmd[0][indexRow].performAction();
				}
			}
			else if (indexRow > -1 && indexRow < cmd[1].Length)
			{
				cmd[1][indexRow].performAction();
			}
		}
		base.updateKey();
		GameCanvas.clearKeyPressed();
	}

	protected void doSelectServer()
	{
		MyVector myVector = new MyVector();
		if (GameMidlet.nameServer != null)
		{
			for (int i = 0; i < GameMidlet.nameServer.Length; i++)
			{
				myVector.addElement(new Command(GameMidlet.nameServer[i], this, 20000 + i, null));
			}
			GameCanvas.menu.startAt(myVector, 0);
			int curInd = loadIndexServer();
			if (curInd >= 0 && curInd < GameMidlet.nameServer.Length)
			{
				GameCanvas.menu.menuSelectedItem = curInd;
			}
		}
	}

	public static void saveIndexServer(int index)
	{
		RMS.saveRMSInt("indServer", index);
	}

	public static int loadIndexServer()
	{
		return RMS.loadRMSInt("indServer");
	}

	public void doViewFAQ()
	{
		if (!listFAQ.Equals(string.Empty) || !listFAQ.Equals(string.Empty))
		{
		}
		if (!Session_ME.connected)
		{
			isFAQ = true;
			GameCanvas.connect(6);
		}
		GameCanvas.startWaitDlg();
	}

	public static bool isVirtualAcc()
	{
		if (uname != null && (uname.StartsWith("tmpusr") || uname.Equals(string.Empty)))
		{
			return true;
		}
		return false;
	}

	public static string randomNumberlist()
	{
		string text = string.Empty;
		for (int i = 0; i < 12; i++)
		{
			string text2 = Res.random(0, 9).ToString();
			text += text2;
		}
		return text;
	}

	public void perform(int idAction, object p)
	{
		if (idAction == 12345)
		{
			MyVector myVector = new MyVector();
			myVector.addElement(new Command("World", this, 123450, null));
			myVector.addElement(new Command("Việt", this, 123451, null));
			GameCanvas.menu.startAt(myVector, 3);
		}
		else if (idAction == 123450)
		{
			MyVector myVector2 = new MyVector();
			string path = Application.persistentDataPath + "/Acc/World";
			if (Directory.Exists(path))
			{
				bool flag = false;
				FileInfo[] files = new DirectoryInfo(path).GetFiles("*");
				foreach (FileInfo fileInfo in files)
				{
					myVector2.addElement(new Command(fileInfo.Name, this, 123452, null));
					flag = true;
				}
				if (!flag)
				{
					GameCanvas.startOKDlg("Hiện không có tài khoản trong danh sách");
				}
				GameCanvas.menu.startAt(myVector2, 3);
			}
			else
			{
				Directory.CreateDirectory(path);
				GameCanvas.startOKDlg("Đã tạo mới thư mục tài khoản");
			}
		}
		else if (idAction == 123452)
		{
			string text = Application.persistentDataPath + "/Acc/World";
			if (Directory.Exists(text))
			{
				try
				{
					FileInfo[] files = new DirectoryInfo(text).GetFiles("*");
					foreach (FileInfo fileInfo2 in files)
					{
						if (((Command)GameCanvas.menu.menuItems.elementAt(GameCanvas.menu.menuSelectedItem)).caption == fileInfo2.Name)
						{
							uname = File.ReadAllText(text + "/" + fileInfo2.Name).Split(',')[0];
							pass = File.ReadAllText(text + "/" + fileInfo2.Name).Split(',')[1];
							RMS.saveRMSString("acc", uname);
							RMS.saveRMSString("pass", pass);
							GameCanvas.selectsvScr.switchToMe();
							return;
						}
					}
				}
				catch (Exception)
				{
					GameCanvas.startYesNoDlg("Lỗi ! bạn có muốn xóa hết dữ liệu không ?", new Command("Có", this, 1, null), new Command("Không", this, 2, null));
					return;
				}
				GameCanvas.startOKDlg("Không có thông tin");
			}
			else
			{
				Directory.CreateDirectory(text);
				GameCanvas.startOKDlg("Đã tạo mới thư mục tài khoản");
			}
		}
		else if (idAction <= 10001)
		{
			switch (idAction)
			{
			case 1000:
				if (isVirtualAcc() && !uname.Equals(string.Empty))
				{
					GameCanvas.startYesNoDlg(mResources.NEW_ACC_ARLET, new Command(mResources.COUNTINUE_PLAY, this, 10001, null), new Command(mResources.NO, GameCanvas.instance, 8882, null));
					break;
				}
				doViewFAQ();
				Service.gI().login("-1", "12345", "1.8.0");
				break;
			case 1001:
				if (isVirtualAcc() && !uname.Equals(string.Empty) && unameChange.Equals(string.Empty))
				{
					GameCanvas.startYesNoDlg(mResources.NEW_ACC_ARLET, new Command(mResources.COUNTINUE, this, 10004, null), new Command(mResources.NO, GameCanvas.instance, 8882, null));
				}
				else
				{
					GameCanvas.loginScr.switchToMe();
				}
				break;
			case 1002:
				doSelectServer();
				break;
			case 1003:
				doViewFAQ();
				if (!unameChange.Equals(string.Empty))
				{
					uname = unameChange;
					pass = passChange;
					unameChange = string.Empty;
					passChange = string.Empty;
					RMS.saveRMSString("acc", uname);
					RMS.saveRMSString("pass", pass);
				}
				Service.gI().login(uname, pass, "1.8.0");
				break;
			case 10001:
				doViewFAQ();
				Service.gI().login("-1", "12345", "1.8.0");
				if (!unameChange.Equals(string.Empty))
				{
					uname = unameChange;
					pass = passChange;
					unameChange = string.Empty;
					passChange = string.Empty;
					RMS.saveRMSString("acc", uname);
					RMS.saveRMSString("pass", pass);
				}
				break;
			}
		}
		else if (idAction != 10004)
		{
			if (idAction >= 20000 && GameMidlet.nameServer != null && idAction < 20000 + GameMidlet.nameServer.Length)
			{
				if (Session_ME.gI().isConnected())
				{
					Session_ME.gI().close();
				}
				int num = idAction - 20000;
				GameCanvas.menu.showMenu = false;
				GameMidlet.IP = GameMidlet.ipList[num];
				GameMidlet.PORT = GameMidlet.portList[num];
				GameMidlet.serverLogin = GameMidlet.serverLoginList[num];
				saveIndexServer(num);
				if (GameMidlet.serverST != null && num < GameMidlet.serverST.Length)
				{
					GameCanvas.menu.menuSelectedItem = GameMidlet.serverST[num];
				}
				GameCanvas.connect(7);
			}
		}
		else
		{
			GameCanvas.currentDialog = null;
			GameCanvas.loginScr.switchToMe();
		}
	}

	static SelectServerScr()
	{
		unameChange = string.Empty;
		passChange = string.Empty;
	}
}
