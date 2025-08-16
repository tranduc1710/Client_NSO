using System;
using UnityEngine;

public class Controller : IMessageHandler
{
	protected static Controller me;

	public Message messWait;

	private int move;

	private int total;

	public const int ID_NEWMOB = 236;

	public static int map;

	public static Controller gI()
	{
		if (me == null)
		{
			me = new Controller();
		}
		return me;
	}

	public void onConnectOK()
	{
		Out.println("Connect ok");
	}

	public void onConnectionFail()
	{
		GameCanvas.isConnectFail = true;
	}

	public void onDisconnected()
	{
		GameCanvas.instance.resetToLoginScr();
	}

	public void requestItemPlayer(Message msg)
	{
		try
		{
			int num = msg.reader().readUnsignedByte();
			Item item = GameScr.currentCharViewInfo.arrItemBody[num];
			item.expires = msg.reader().readLong();
			item.saleCoinLock = msg.reader().readInt();
			item.sys = msg.reader().readByte();
			item.options = new MyVector();
			try
			{
				while (true)
				{
					item.options.addElement(new ItemOption(msg.reader().readUnsignedByte(), msg.reader().readInt()));
				}
			}
			catch (Exception ex)
			{
				Out.println(" >>>11  loi tai requestItemPlayer" + ex.ToString());
			}
		}
		catch (Exception ex2)
		{
			Out.println(">>>222 loi tai requestItemPlayer" + ex2.ToString());
		}
	}

	public void viewItemAuction(Message msg)
	{
		try
		{
			Item item = null;
			int num = msg.reader().readInt();
			for (int i = 0; i < GameScr.arrItemStands.Length; i++)
			{
				if (GameScr.arrItemStands[i].item.itemId == num)
				{
					item = GameScr.arrItemStands[i].item;
					break;
				}
			}
			item.typeUI = 37;
			item.expires = -1L;
			item.saleCoinLock = msg.reader().readInt();
			if (!item.isTypeBody() && !item.isTypeNgocKham())
			{
				return;
			}
			item.options = new MyVector();
			try
			{
				item.upgrade = msg.reader().readByte();
				item.sys = msg.reader().readByte();
				while (true)
				{
					item.options.addElement(new ItemOption(msg.reader().readUnsignedByte(), msg.reader().readInt()));
				}
			}
			catch (Exception)
			{
			}
		}
		catch (Exception)
		{
		}
	}

	public void onMessage(Message msg)
	{
		GameCanvas.debugSession.removeAllElements();
		GameCanvas.debug("SA1", 2);
		Mob mob = null;
		try
		{
			switch (msg.command)
			{
			case -30:
				messageSubCommand(msg);
				break;
			case -29:
				messageNotLogin(msg);
				break;
			case -28:
				messageNotMap(msg);
				break;
			case -26:
				GameCanvas.debug("SA2", 2);
				GameCanvas.startOKDlg(msg.reader().readUTF());
				break;
			case -25:
			{
				GameCanvas.debug("SA3", 2);
				string text5 = msg.reader().readUTF();
				Info.addInfo(text5, 150, mFont.tahoma_7b_yellow);
				ChatManager.gI().addChat(mResources.GLOBALCHAT[0], mResources.SERVER_ALERT, text5);
				break;
			}
			case -24:
			{
				GameCanvas.debug("SA3", 2);
				string text15 = msg.reader().readUTF();
				InfoMe.addInfo(text15, 50, mFont.tahoma_7_yellow);
				if (text15.Contains("Vật phẩm của người khác"))
				{
					ItemMap.d();
					if (Char.getMyChar().itemFocus != null)
					{
						Char.getMyChar().itemFocus.isWait = true;
					}
				}
				break;
			}
			case -23:
			{
				GameCanvas.debug("SA91", 2);
				int num81 = msg.reader().readInt();
				string text14 = msg.reader().readUTF();
				Char char23 = ((Char.getMyChar().charID != num81) ? GameScr.findCharInMap(num81) : Char.getMyChar());
				if (char23 == null)
				{
					return;
				}
				ChatPopup.addChatPopup(text14, 100, char23);
				ChatManager.gI().addChat(mResources.PUBLICCHAT[0], char23.cName, text14);
				break;
			}
			case -22:
			{
				string text11 = msg.reader().readUTF();
				string text12 = msg.reader().readUTF();
				ChatManager.gI().addChat(text11, text11, text12);
				if ((!GameScr.isPaintMessage || !ChatManager.gI().getCurrentChatTab().ownerName.Equals(text11)) && !ChatManager.blockPrivateChat)
				{
					ChatManager.gI().addWaitList(text11);
				}
				break;
			}
			case -21:
			{
				string text3 = msg.reader().readUTF();
				string text4 = msg.reader().readUTF();
				ChatManager.gI().addChat(mResources.GLOBALCHAT[0], text3, text4);
				if (!ChatManager.blockGlobalChat)
				{
					Info.addInfo(text3 + ": " + text4, 80, mFont.tahoma_7b_yellow);
				}
				break;
			}
			case -20:
			{
				string whoChat2 = msg.reader().readUTF();
				string text16 = msg.reader().readUTF();
				ChatManager.gI().addChat(mResources.PARTYCHAT[0], whoChat2, text16);
				Buff.PartyTo(text16);
				if (!GameScr.isPaintMessage || ChatManager.gI().getCurrentChatTab().type != 1)
				{
					ChatManager.isMessagePt = true;
				}
				break;
			}
			case -19:
			{
				string whoChat = msg.reader().readUTF();
				string text10 = msg.reader().readUTF();
				ChatManager.gI().addChat(mResources.CLANCHAT[0], whoChat, text10);
				if (!GameScr.isPaintMessage || ChatManager.gI().getCurrentChatTab().type != 4)
				{
					ChatManager.isMessageClan = true;
				}
				break;
			}
			case -18:
				GameCanvas.isLoading = true;
				GameScr.resetAllvector();
				TileMap.vGo.removeAllElements();
				TileMap.mapID = msg.reader().readUnsignedByte();
				TileMap.tileID = msg.reader().readByte();
				TileMap.bgID = msg.reader().readByte();
				TileMap.typeMap = msg.reader().readByte();
				TileMap.mapName = msg.reader().readUTF();
				TileMap.zoneID = msg.reader().readByte();
				try
				{
					TileMap.loadMapFromResource(TileMap.mapID);
				}
				catch (Exception)
				{
					Out.println("load map from server: " + TileMap.mapID);
					Service.gI().requestMaptemplate(TileMap.mapID);
					messWait = msg;
					return;
				}
				Resources.UnloadUnusedAssets();
				GC.Collect();
				loadInfoMap(msg);
				if (Char.getMyChar().mobMe != null)
				{
					Char.getMyChar().mobMe.x = Char.getMyChar().cx;
					Char.getMyChar().mobMe.y = Char.getMyChar().cy - 40;
				}
				break;
			case -16:
				GameCanvas.debug("SA65", 2);
				Char.isLockKey = true;
				Char.ischangingMap = true;
				Mob.vEggMonter.removeAllElements();
				if (!Main.isPC)
				{
					GameCanvas.startWaitDlgIpad(mResources.PLEASEWAIT, isIpad: true);
				}
				GameScr.gI().timeStartMap = 0;
				GameScr.gI().timeLengthMap = 0;
				Char.getMyChar().mobFocus = null;
				Char.getMyChar().npcFocus = null;
				Char.getMyChar().charFocus = null;
				Char.getMyChar().itemFocus = null;
				Char.getMyChar().focus.removeAllElements();
				Char.getMyChar().testCharId = -9999;
				Char.getMyChar().killCharId = -9999;
				GameScr.resetAllvector();
				GameCanvas.resetBg();
				if (GameScr.vParty.size() <= 1)
				{
					GameScr.vParty.removeAllElements();
				}
				GameScr.gI().resetButton();
				GameScr.gI().center = null;
				break;
			case -15:
			{
				GameCanvas.debug("SA60", 2);
				short num107 = msg.reader().readShort();
				for (int num108 = 0; num108 < GameScr.vItemMap.size(); num108++)
				{
					ItemMap itemMap5 = (ItemMap)GameScr.vItemMap.elementAt(num108);
					if (itemMap5 != null && itemMap5.itemMapID == num107)
					{
						GameScr.vItemMap.removeElementAt(num108);
						break;
					}
				}
				break;
			}
			case -14:
			{
				GameCanvas.debug("SA61", 2);
				Char.getMyChar().itemFocus = null;
				short num101 = msg.reader().readShort();
				for (int num102 = 0; num102 < GameScr.vItemMap.size(); num102++)
				{
					ItemMap itemMap4 = (ItemMap)GameScr.vItemMap.elementAt(num102);
					if (itemMap4.itemMapID != num101)
					{
						continue;
					}
					itemMap4.setPoint(Char.getMyChar().cx, Char.getMyChar().cy - 10);
					if (itemMap4.template.type == 19)
					{
						int num103 = msg.reader().readUnsignedShort();
						Char.getMyChar().yen += num103;
						if (itemMap4.template.id != 238)
						{
							InfoMe.addInfo(mResources.RECEIVE + " " + num103 + " " + mResources.YEN);
						}
					}
					else if (itemMap4.template.type == 25 && itemMap4.template.id != 238)
					{
						InfoMe.addInfo(mResources.RECEIVE + " " + itemMap4.template.name, 15, mFont.tahoma_7_yellow);
					}
					break;
				}
				break;
			}
			case -13:
			{
				GameCanvas.debug("SA62", 2);
				short num95 = msg.reader().readShort();
				for (int num96 = 0; num96 < GameScr.vItemMap.size(); num96++)
				{
					ItemMap itemMap3 = (ItemMap)GameScr.vItemMap.elementAt(num96);
					if (itemMap3 == null || itemMap3.itemMapID != num95)
					{
						continue;
					}
					Char char27 = GameScr.findCharInMap(msg.reader().readInt());
					if (char27 == null)
					{
						return;
					}
					itemMap3.setPoint(char27.cx, char27.cy - 10);
					if (itemMap3.x < char27.cx)
					{
						char27.cdir = -1;
					}
					else if (itemMap3.x > char27.cx)
					{
						char27.cdir = 1;
					}
					break;
				}
				break;
			}
			case -12:
			{
				GameCanvas.debug("SA63", 2);
				int num97 = msg.reader().readByte();
				GameScr.vItemMap.addElement(new ItemMap(msg.reader().readShort(), Char.getMyChar().arrItemBag[num97].template.id, Char.getMyChar().cx, Char.getMyChar().cy, msg.reader().readShort(), msg.reader().readShort()));
				Char.getMyChar().arrItemBag[num97] = null;
				break;
			}
			case -11:
				GameCanvas.debug("SA88", 2);
				Char.getMyChar().cPk = msg.reader().readByte();
				Char.getMyChar().waitToDie(msg.reader().readShort(), msg.reader().readShort());
				try
				{
					Char.getMyChar().cEXP = msg.reader().readLong();
					GameScr.setLevel_Exp(Char.getMyChar().cEXP, value: true);
				}
				catch (Exception)
				{
				}
				Char.getMyChar().countKill = 0;
				break;
			case -10:
				GameCanvas.debug("SA90", 2);
				if (Char.getMyChar().wdx != 0 || Char.getMyChar().wdy != 0)
				{
					Char.getMyChar().cx = Char.getMyChar().wdx;
					Char.getMyChar().cy = Char.getMyChar().wdy;
					Char.getMyChar().wdx = (Char.getMyChar().wdy = 0);
				}
				Char.getMyChar().liveFromDead();
				Char.isLockKey = false;
				break;
			case -8:
			{
				GameCanvas.debug("SA77", 22);
				int num32 = msg.reader().readInt();
				Char.getMyChar().yen += num32;
				GameScr.gI().yenTemp = num32;
				InfoMe.addInfo(mResources.RECEIVE + " " + num32 + " " + mResources.YEN);
				GameScr.startFlyText((num32 <= 0) ? (string.Empty + num32) : ("+" + num32), Char.getMyChar().cx, Char.getMyChar().cy - Char.getMyChar().ch - 10, 0, -2, mFont.YELLOW);
				break;
			}
			case -7:
			{
				GameCanvas.debug("SA77", 222);
				int num42 = msg.reader().readInt();
				Char.getMyChar().xu += num42;
				Char.getMyChar().yen -= num42;
				GameScr.startFlyText("+" + num42, Char.getMyChar().cx, Char.getMyChar().cy - Char.getMyChar().ch - 10, 0, -2, mFont.YELLOW);
				break;
			}
			case -6:
			{
				GameCanvas.debug("SA64", 2);
				Char char7 = GameScr.findCharInMap(msg.reader().readInt());
				if (char7 == null)
				{
					return;
				}
				GameScr.vItemMap.addElement(new ItemMap(msg.reader().readShort(), msg.reader().readShort(), char7.cx, char7.cy, msg.reader().readShort(), msg.reader().readShort()));
				break;
			}
			case -5:
				GameCanvas.debug("SA82", 2);
				try
				{
					mob = Mob.get_Mob(msg.reader().readUnsignedByte());
					mob.sys = msg.reader().readByte();
					mob.levelBoss = msg.reader().readByte();
					mob.x = mob.xFirst;
					mob.y = mob.yFirst;
					mob.status = 5;
					mob.injureThenDie = false;
					mob.hp = msg.reader().readInt();
					mob.maxHp = mob.hp;
					if (mob.getTemplate().mobTemplateId == 202)
					{
						ServerEffect.addServerEffect(148, mob.x, mob.y, 0);
					}
					else
					{
						ServerEffect.addServerEffect(60, mob.x, mob.y, 1);
					}
				}
				catch (Exception)
				{
				}
				break;
			case -3:
				GameCanvas.debug("SA86", 2);
				mob = null;
				try
				{
					mob = Mob.get_Mob(msg.reader().readUnsignedByte());
				}
				catch (Exception)
				{
				}
				if (mob != null)
				{
					int num116 = msg.reader().readInt();
					int num117;
					try
					{
						num117 = msg.reader().readInt();
					}
					catch (Exception)
					{
						num117 = 0;
					}
					if (mob.isBusyAttackSomeOne)
					{
						Char.getMyChar().doInjure(num116, num117, isBoss: false, -1);
						mob.attackOtherInRange();
					}
					else
					{
						mob.dame = num116;
						mob.dameMp = num117;
						mob.setAttack(Char.getMyChar());
					}
					short idSkill_atk3 = msg.reader().readShort();
					sbyte typeAtk3 = msg.reader().readByte();
					sbyte typeTool3 = msg.reader().readByte();
					mob.setTypeAtk(idSkill_atk3, typeAtk3, typeTool3);
				}
				break;
			case -1:
			{
				GameCanvas.debug("SA83", 2);
				mob = null;
				try
				{
					mob = Mob.get_Mob(msg.reader().readUnsignedByte());
				}
				catch (Exception)
				{
				}
				GameCanvas.debug("SA83v1", 2);
				if (mob == null)
				{
					break;
				}
				mob.hp = msg.reader().readInt();
				int num83 = msg.reader().readInt();
				if (num83 < 0)
				{
					num83 = Res.abs(num83) + 32767;
				}
				bool flag2 = msg.reader().readBoolean();
				try
				{
					if (msg.reader().available() > 0)
					{
						mob.levelBoss = msg.reader().readByte();
					}
					mob.maxHp = msg.reader().readInt();
				}
				catch (Exception)
				{
				}
				if (flag2)
				{
					GameScr.startFlyText("-" + num83, mob.x, mob.y - mob.h, 0, -2, mFont.FATAL);
				}
				else
				{
					GameScr.startFlyText("-" + num83, mob.x, mob.y - mob.h, 0, -2, mFont.ORANGE);
				}
				break;
			}
			case 0:
			{
				GameCanvas.debug("SA89", 2);
				Char char19 = GameScr.findCharInMap(msg.reader().readInt());
				if (char19 == null)
				{
					return;
				}
				char19.cPk = msg.reader().readByte();
				if (char19.charID == Char.aCID)
				{
					Char.isAFocusDie = true;
				}
				char19.waitToDie(msg.reader().readShort(), msg.reader().readShort());
				if (Char.getMyChar().charFocus == char19)
				{
					Char.getMyChar().charFocus = null;
				}
				break;
			}
			case 1:
			{
				GameCanvas.debug("SA80", 2);
				int num29 = msg.reader().readInt();
				for (int num30 = 0; num30 < GameScr.vCharInMap.size(); num30++)
				{
					Char char11 = null;
					try
					{
						char11 = (Char)GameScr.vCharInMap.elementAt(num30);
					}
					catch (Exception)
					{
					}
					if (char11 == null)
					{
						break;
					}
					if (char11.charID == num29)
					{
						GameCanvas.debug("SA8x2y" + num30, 2);
						char11.cxMoveLast = msg.reader().readShort();
						char11.cyMoveLast = msg.reader().readShort();
						char11.moveTo(char11.cxMoveLast, char11.cyMoveLast);
						char11.lastUpdateTime = mSystem.getCurrentTimeMillis();
						break;
					}
				}
				GameCanvas.debug("SA80x3", 2);
				break;
			}
			case 2:
			{
				GameCanvas.debug("SA81", 2);
				int num7 = msg.reader().readInt();
				for (int j = 0; j < GameScr.vCharInMap.size(); j++)
				{
					Char char9 = (Char)GameScr.vCharInMap.elementAt(j);
					if (char9 != null && char9.charID == num7)
					{
						if (!char9.isInvisible)
						{
							ServerEffect.addServerEffect(60, char9.cx, char9.cy, 1);
						}
						GameScr.vCharInMap.removeElementAt(j);
						Party.clear(num7);
						return;
					}
				}
				break;
			}
			case 3:
			{
				GameCanvas.debug("SA79", 2);
				Char char12 = new Char();
				char12.charID = msg.reader().readInt();
				if (readCharInfo(char12, msg))
				{
					GameScr.vCharInMap.addElement(char12);
				}
				break;
			}
			case 4:
			{
				GameCanvas.debug("SA76", 2);
				Char char21 = GameScr.findCharInMap(msg.reader().readInt());
				if (char21 == null)
				{
					return;
				}
				GameCanvas.debug("SA76v1", 2);
				if ((TileMap.tileTypeAtPixel(char21.cx, char21.cy) & TileMap.T_TOP) == TileMap.T_TOP)
				{
					char21.setSkillPaint(GameScr.sks[msg.reader().readByte()], 0);
				}
				else
				{
					char21.setSkillPaint(GameScr.sks[msg.reader().readByte()], 1);
				}
				if (char21.isWolf)
				{
					char21.isWolf = false;
					char21.timeSummon = mSystem.currentTimeMillis();
					if (char21.vitaWolf >= 500)
					{
						ServerEffect.addServerEffect(60, char21, 1);
					}
				}
				if (char21.isMoto)
				{
					char21.isMoto = false;
					char21.isMotoBehind = true;
				}
				GameCanvas.debug("SA76v2", 2);
				int num73 = msg.reader().readByte();
				char21.attMobs = new Mob[num73];
				for (int num74 = 0; num74 < char21.attMobs.Length; num74++)
				{
					mob = Mob.get_Mob(msg.reader().readUnsignedByte());
					char21.attMobs[num74] = mob;
					if (num74 == 0)
					{
						if (char21.cx <= mob.x)
						{
							char21.cdir = 1;
						}
						else
						{
							char21.cdir = -1;
						}
					}
				}
				GameCanvas.debug("SA76v3", 2);
				char21.mobFocus = char21.attMobs[0];
				Char[] array5 = new Char[10];
				int num75 = 0;
				try
				{
					for (num75 = 0; num75 < array5.Length; num75++)
					{
						int num76 = msg.reader().readInt();
						Char char22 = (array5[num75] = ((num76 != Char.getMyChar().charID) ? GameScr.findCharInMap(num76) : Char.getMyChar()));
						if (num75 == 0)
						{
							if (char21.cx <= char22.cx)
							{
								char21.cdir = 1;
							}
							else
							{
								char21.cdir = -1;
							}
						}
					}
				}
				catch (Exception)
				{
				}
				GameCanvas.debug("SA76v4", 2);
				if (num75 > 0)
				{
					char21.attChars = new Char[num75];
					for (num75 = 0; num75 < char21.attChars.Length; num75++)
					{
						char21.attChars[num75] = array5[num75];
					}
					char21.charFocus = char21.attChars[0];
				}
				GameCanvas.debug("SA76v5", 2);
				break;
			}
			case 5:
			{
				GameCanvas.debug("SA78", 2);
				long num70 = msg.reader().readLong();
				Char.getMyChar().cExpDown = 0L;
				Char.getMyChar().cEXP += num70;
				int clevel = Char.getMyChar().clevel;
				GameScr.setLevel_Exp(Char.getMyChar().cEXP, value: true);
				if (clevel != Char.getMyChar().clevel)
				{
					ServerEffect.addServerEffect(58, Char.getMyChar(), 1);
				}
				GameScr.startFlyText("+" + num70, Char.getMyChar().cx, Char.getMyChar().cy - Char.getMyChar().ch, 0, -2, mFont.GREEN);
				if (num70 >= 1000000)
				{
					InfoMe.addInfo(mResources.RECEIVE + " " + num70 + " " + mResources.EXP, 20, mFont.tahoma_7_yellow);
				}
				break;
			}
			case 6:
			{
				GameCanvas.debug("SA6333", 2);
				ItemMap itemMap2 = new ItemMap(msg.reader().readShort(), msg.reader().readShort(), msg.reader().readShort(), msg.reader().readShort());
				sbyte[] array4 = NinjaUtil.readByteArray_Int(msg);
				if (array4 != null && array4.Length != 0)
				{
					itemMap2.imgCaptcha = new MyImage();
					itemMap2.imgCaptcha.img = createImage(array4);
				}
				GameScr.vItemMap.addElement(itemMap2);
				break;
			}
			case 7:
				GameCanvas.debug("SA633355", 2);
				Char.getMyChar().arrItemBag[msg.reader().readByte()].quantity = msg.reader().readShort();
				break;
			case 8:
			{
				GameCanvas.debug("SA37", 2);
				int num100 = msg.reader().readByte();
				Char.getMyChar().arrItemBag[num100] = new Item();
				Char.getMyChar().arrItemBag[num100].typeUI = 3;
				Char.getMyChar().arrItemBag[num100].indexUI = num100;
				Char.getMyChar().arrItemBag[num100].template = ItemTemplates.get(msg.reader().readShort());
				Char.getMyChar().arrItemBag[num100].isLock = msg.reader().readBoolean();
				if (Char.getMyChar().arrItemBag[num100].isTypeBody() || Char.getMyChar().arrItemBag[num100].isTypeNgocKham())
				{
					Char.getMyChar().arrItemBag[num100].upgrade = msg.reader().readByte();
				}
				Char.getMyChar().arrItemBag[num100].isExpires = msg.reader().readBoolean();
				try
				{
					Char.getMyChar().arrItemBag[num100].quantity = msg.reader().readUnsignedShort();
				}
				catch (Exception)
				{
					Char.getMyChar().arrItemBag[num100].quantity = 1;
				}
				if (Char.getMyChar().arrItemBag[num100].template.type == 16)
				{
					GameScr.hpPotion += Char.getMyChar().arrItemBag[num100].quantity;
				}
				if (Char.getMyChar().arrItemBag[num100].template.type == 17)
				{
					GameScr.mpPotion += Char.getMyChar().arrItemBag[num100].quantity;
				}
				if (Char.getMyChar().arrItemBag[num100].template.id == 340)
				{
					GameScr.gI().numSprinLeft += Char.getMyChar().arrItemBag[num100].quantity;
				}
				CLock.HuyLockBuy();
				if (GameScr.isPaintTrade)
				{
					if (GameScr.gI().tradeItemName.Equals(string.Empty))
					{
						GameScr.gI().tradeItemName += Char.getMyChar().arrItemBag[num100].template.name;
						break;
					}
					GameScr gameScr3 = GameScr.gI();
					gameScr3.tradeItemName = gameScr3.tradeItemName + ", " + Char.getMyChar().arrItemBag[num100].template.name;
				}
				else if (Char.getMyChar().arrItemBag[num100].template.type != 20)
				{
					InfoMe.addInfo(mResources.RECEIVE + " " + Char.getMyChar().arrItemBag[num100].template.name);
				}
				break;
			}
			case 9:
			{
				GameCanvas.debug("SA39", 2);
				Item item4 = Char.getMyChar().arrItemBag[msg.reader().readUnsignedByte()];
				int num78 = 0;
				try
				{
					num78 = msg.reader().readShort();
				}
				catch (Exception)
				{
					num78 = 1;
				}
				item4.quantity += num78;
				if (item4.template.type == 16)
				{
					GameScr.hpPotion += num78;
				}
				if (item4.template.type == 17)
				{
					GameScr.mpPotion += num78;
				}
				if (item4.template.id == 340)
				{
					GameScr.gI().numSprinLeft += num78;
				}
				GameCanvas.endDlg();
				if (GameScr.isPaintTrade)
				{
					if (GameScr.gI().tradeItemName.Equals(string.Empty))
					{
						GameScr.gI().tradeItemName += item4.template.name;
						break;
					}
					GameScr gameScr2 = GameScr.gI();
					gameScr2.tradeItemName = gameScr2.tradeItemName + ", " + item4.template.name;
				}
				else if (item4.template.type != 20)
				{
					InfoMe.addInfo(mResources.RECEIVE + " " + item4.template.name);
				}
				break;
			}
			case 10:
			{
				GameCanvas.debug("SA38", 2);
				int num48 = msg.reader().readByte();
				if (Char.getMyChar().arrItemBag[num48].template.type == 16)
				{
					GameScr.hpPotion -= Char.getMyChar().arrItemBag[num48].quantity;
				}
				if (Char.getMyChar().arrItemBag[num48].template.type == 17)
				{
					GameScr.mpPotion -= Char.getMyChar().arrItemBag[num48].quantity;
				}
				Char.getMyChar().arrItemBag[num48] = null;
				if (GameScr.gI().isPaintUI())
				{
					GameScr.gI().left = (GameScr.gI().center = null);
				}
				else
				{
					GameScr.gI().resetButton();
				}
				break;
			}
			case 11:
			{
				int num56 = msg.reader().readByte();
				if (Char.getMyChar().arrItemBag[num56].template.type == 24)
				{
					InfoDlg.hide();
				}
				Char.getMyChar().useItem(num56);
				Char.getMyChar().readParam(msg, "Cmd.ITEM_USE");
				Char.getMyChar().eff5BuffHp = msg.reader().readShort();
				Char.getMyChar().eff5BuffMp = msg.reader().readShort();
				GameScr.gI().setLCR();
				break;
			}
			case 13:
				GameCanvas.debug("SA70", 2);
				Char.getMyChar().xu = msg.reader().readInt();
				Char.getMyChar().yen = msg.reader().readInt();
				Char.getMyChar().luong = msg.reader().readInt();
				GameCanvas.endDlg();
				break;
			case 14:
			{
				Item item2 = Char.getMyChar().arrItemBag[msg.reader().readByte()];
				Char.getMyChar().yen = msg.reader().readInt();
				int num6 = 0;
				try
				{
					num6 = msg.reader().readShort();
				}
				catch (Exception)
				{
					num6 = 1;
				}
				item2.quantity -= num6;
				if (item2.template.type == 16)
				{
					GameScr.hpPotion -= num6;
				}
				if (item2.template.type == 17)
				{
					GameScr.mpPotion -= num6;
				}
				if (item2.quantity <= 0)
				{
					Char.getMyChar().arrItemBag[item2.indexUI] = null;
				}
				GameScr.gI().left = (GameScr.gI().center = null);
				GameScr.gI().updateCommandForUI();
				GameCanvas.endDlg();
				break;
			}
			case 15:
				GameCanvas.debug("SA40", 2);
				Char.getMyChar().itemBodyToBag(msg);
				break;
			case 16:
				GameCanvas.debug("SA41", 2);
				Char.getMyChar().itemBoxToBag(msg);
				break;
			case 17:
				GameCanvas.debug("SA42", 2);
				Char.getMyChar().itemBagToBox(msg);
				break;
			case 18:
			{
				GameCanvas.debug("SYA9", 2);
				int num98 = msg.reader().readByte();
				int num99 = 1;
				try
				{
					num99 = msg.reader().readShort();
				}
				catch (Exception)
				{
				}
				if (Char.getMyChar().arrItemBag[num98].template.type == 24)
				{
					InfoDlg.hide();
				}
				if (Char.getMyChar().arrItemBag[num98].template.type == 16)
				{
					GameScr.hpPotion--;
				}
				if (Char.getMyChar().arrItemBag[num98].template.type == 17)
				{
					GameScr.mpPotion--;
				}
				if (Char.getMyChar().arrItemBag[num98].quantity > num99)
				{
					Char.getMyChar().arrItemBag[num98].quantity -= num99;
				}
				else
				{
					Char.getMyChar().arrItemBag[num98] = null;
				}
				if (GameScr.isPaintInfoMe)
				{
					GameScr.gI().setLCR();
				}
				break;
			}
			case 19:
				GameCanvas.debug("SA43", 2);
				Char.getMyChar().crystalCollect(msg, isCoin: true);
				break;
			case 20:
				GameCanvas.debug("SA44", 2);
				Char.getMyChar().crystalCollect(msg, isCoin: false);
				break;
			case 21:
			{
				GameCanvas.debug("SA45", 2);
				int num60 = msg.reader().readByte();
				Char.getMyChar().luong = msg.reader().readInt();
				Char.getMyChar().xu = msg.reader().readInt();
				Char.getMyChar().yen = msg.reader().readInt();
				if (GameScr.itemUpGrade != null)
				{
					GameScr.itemUpGrade.upgrade = msg.reader().readByte();
					GameScr.itemUpGrade.isLock = true;
					GameScr.itemUpGrade.clearExpire();
					if (num60 == 1)
					{
						GameScr.effUpok = GameScr.efs[53];
						GameScr.indexEff = 0;
					}
				}
				if (GameScr.arrItemUpGrade != null)
				{
					for (int num61 = 0; num61 < GameScr.arrItemUpGrade.Length; num61++)
					{
						GameScr.arrItemUpGrade[num61] = null;
					}
				}
				if (num60 == 5 || num60 == 6)
				{
					if (GameScr.itemSplit != null)
					{
						GameScr.itemSplit = null;
					}
					if (GameScr.arrItemSplit != null)
					{
						for (int num62 = 0; num62 < GameScr.arrItemSplit.Length; num62++)
						{
							GameScr.arrItemSplit[num62] = null;
						}
					}
				}
				GameScr.gI().left = (GameScr.gI().center = null);
				GameScr.gI().updateKeyBuyItemUI();
				GameCanvas.endDlg();
				switch (num60)
				{
				case 1:
					InfoMe.addInfo(mResources.TYPEUPGRADE[0] + GameScr.itemUpGrade.upgrade, 20, mFont.tahoma_7_white);
					break;
				case 5:
					InfoMe.addInfo(mResources.TYPEKHAMNGOC[0] + GameScr.itemUpGrade.upgrade, 20, mFont.tahoma_7_white);
					break;
				default:
					InfoMe.addInfo(mResources.TYPEUPGRADE[1] + GameScr.itemUpGrade.upgrade, 20, mFont.tahoma_7_red);
					break;
				case 6:
					InfoMe.addInfo(mResources.TYPEKHAMNGOC[1] + GameScr.itemUpGrade.upgrade, 20, mFont.tahoma_7_red);
					break;
				}
				break;
			}
			case 22:
			{
				GameCanvas.debug("SA46", 2);
				int num36 = msg.reader().readByte();
				string text8 = mResources.SPLIT_ITEM_NAME;
				for (int num37 = 0; num37 < GameScr.arrItemSplit.Length; num37++)
				{
					GameScr.arrItemSplit[num37] = null;
				}
				for (int num38 = 0; num38 < num36; num38++)
				{
					Item item3 = new Item();
					item3.typeUI = 3;
					item3.indexUI = msg.reader().readByte();
					item3.template = ItemTemplates.get(msg.reader().readShort());
					item3.expires = -1L;
					item3.quantity = 1;
					item3.isLock = GameScr.itemSplit.isLock;
					Char.getMyChar().arrItemBag[item3.indexUI] = item3;
					text8 += item3.template.name;
					if (num38 < num36 - 1)
					{
						text8 += ", ";
					}
				}
				GameScr.itemSplit.upgrade = 0;
				GameScr.itemSplit.clearExpire();
				GameScr.gI().left = (GameScr.gI().center = null);
				GameScr.gI().updateCommandForUI();
				GameCanvas.endDlg();
				InfoMe.addInfo(text8);
				GameScr.effUpok = GameScr.efs[66];
				GameScr.indexEff = 0;
				break;
			}
			case 23:
			{
				GameCanvas.debug("SXX7", 2);
				string text7 = msg.reader().readUTF();
				GameCanvas.startYesNoDlg(text7 + " " + mResources.PLEASE_PARTY, 8889, text7, 8882, null);
				break;
			}
			case 25:
			{
				sbyte b4 = msg.reader().readByte();
				for (int num41 = 0; num41 < b4; num41++)
				{
					int charId2 = msg.reader().readInt();
					int cx = msg.reader().readShort();
					int cy = msg.reader().readShort();
					int hPShow = msg.reader().readInt();
					Char char14 = GameScr.findCharInMap(charId2);
					if (char14 != null)
					{
						char14.cx = cx;
						char14.cy = cy;
						char14.cHP = (char14.HPShow = hPShow);
						char14.lastUpdateTime = mSystem.getCurrentTimeMillis();
					}
				}
				break;
			}
			case 26:
				Char.getMyChar().countKill = msg.reader().readUnsignedShort();
				Char.getMyChar().countKillMax = msg.reader().readUnsignedShort();
				break;
			case 27:
			{
				GameCanvas.debug("SZ7", 2);
				Mob mob2 = Mob.get_Mob(msg.reader().readUnsignedByte());
				int num5 = msg.reader().readInt();
				if (mob2 != null)
				{
					Char char6 = ((num5 != Char.getMyChar().charID) ? GameScr.findCharInMap(num5) : Char.getMyChar());
					if (char6 != null)
					{
						char6.moveFast = new short[3];
						char6.moveFast[0] = 0;
						char6.moveFast[1] = (short)mob2.x;
						char6.moveFast[2] = (short)mob2.y;
					}
				}
				break;
			}
			case 30:
			{
				sbyte typeUI = msg.reader().readByte();
				try
				{
					GameScr.svTitle = msg.reader().readUTF();
					GameScr.svAction = msg.reader().readUTF();
				}
				catch (Exception)
				{
				}
				GameScr.gI().doOpenUI(typeUI);
				break;
			}
			case 31:
			{
				GameCanvas.debug("SA69", 2);
				Char.getMyChar().xuInBox = msg.reader().readInt();
				Char.getMyChar().arrItemBox = new Item[msg.reader().readUnsignedByte()];
				for (int i = 0; i < Char.getMyChar().arrItemBox.Length; i++)
				{
					short num4 = msg.reader().readShort();
					if (num4 != -1)
					{
						Char.getMyChar().arrItemBox[i] = new Item();
						Char.getMyChar().arrItemBox[i].typeUI = 4;
						Char.getMyChar().arrItemBox[i].indexUI = i;
						Char.getMyChar().arrItemBox[i].template = ItemTemplates.get(num4);
						Char.getMyChar().arrItemBox[i].isLock = msg.reader().readBoolean();
						if (Char.getMyChar().arrItemBox[i].isTypeBody() || Char.getMyChar().arrItemBox[i].isTypeNgocKham())
						{
							Char.getMyChar().arrItemBox[i].upgrade = msg.reader().readByte();
						}
						Char.getMyChar().arrItemBox[i].isExpires = msg.reader().readBoolean();
						Char.getMyChar().arrItemBox[i].quantity = msg.reader().readShort();
					}
				}
				CLock.HuyLockTypeUI(4);
				break;
			}
			case 33:
			{
				GameCanvas.debug("SA72", 2);
				sbyte b = msg.reader().readByte();
				switch (b)
				{
				case 14:
				{
					GameScr.arrItemStore = new Item[msg.reader().readByte()];
					for (int num13 = 0; num13 < GameScr.arrItemStore.Length; num13++)
					{
						GameScr.arrItemStore[num13] = new Item();
						GameScr.arrItemStore[num13].typeUI = 14;
						GameScr.arrItemStore[num13].indexUI = msg.reader().readUnsignedByte();
						GameScr.arrItemStore[num13].template = ItemTemplates.get(msg.reader().readShort());
					}
					CLock.HuyLockTypeUI(b);
					break;
				}
				case 15:
				{
					GameScr.arrItemBook = new Item[msg.reader().readByte()];
					for (int num27 = 0; num27 < GameScr.arrItemBook.Length; num27++)
					{
						GameScr.arrItemBook[num27] = new Item();
						GameScr.arrItemBook[num27].typeUI = 15;
						GameScr.arrItemBook[num27].indexUI = msg.reader().readUnsignedByte();
						GameScr.arrItemBook[num27].template = ItemTemplates.get(msg.reader().readShort());
					}
					CLock.HuyLockTypeUI(b);
					break;
				}
				case 32:
				{
					GameScr.arrItemFashion = new Item[msg.reader().readByte()];
					for (int num16 = 0; num16 < GameScr.arrItemFashion.Length; num16++)
					{
						GameScr.arrItemFashion[num16] = new Item();
						GameScr.arrItemFashion[num16].typeUI = 32;
						GameScr.arrItemFashion[num16].indexUI = msg.reader().readUnsignedByte();
						GameScr.arrItemFashion[num16].template = ItemTemplates.get(msg.reader().readShort());
					}
					CLock.HuyLockTypeUI(b);
					break;
				}
				case 34:
				{
					GameScr.arrItemClanShop = new Item[msg.reader().readByte()];
					for (int num22 = 0; num22 < GameScr.arrItemClanShop.Length; num22++)
					{
						GameScr.arrItemClanShop[num22] = new Item();
						GameScr.arrItemClanShop[num22].typeUI = 34;
						GameScr.arrItemClanShop[num22].indexUI = msg.reader().readUnsignedByte();
						GameScr.arrItemClanShop[num22].template = ItemTemplates.get(msg.reader().readShort());
					}
					CLock.HuyLockTypeUI(b);
					break;
				}
				case 35:
				{
					GameScr.arrItemElites = new Item[msg.reader().readByte()];
					for (int m = 0; m < GameScr.arrItemElites.Length; m++)
					{
						GameScr.arrItemElites[m] = new Item();
						GameScr.arrItemElites[m].typeUI = 35;
						GameScr.arrItemElites[m].indexUI = msg.reader().readUnsignedByte();
						GameScr.arrItemElites[m].template = ItemTemplates.get(msg.reader().readShort());
					}
					CLock.HuyLockTypeUI(b);
					break;
				}
				case 20:
				{
					GameScr.arrItemNonNam = new Item[msg.reader().readByte()];
					for (int num21 = 0; num21 < GameScr.arrItemNonNam.Length; num21++)
					{
						GameScr.arrItemNonNam[num21] = new Item();
						GameScr.arrItemNonNam[num21].typeUI = b;
						GameScr.arrItemNonNam[num21].indexUI = msg.reader().readUnsignedByte();
						GameScr.arrItemNonNam[num21].template = ItemTemplates.get(msg.reader().readShort());
					}
					CLock.HuyLockTypeUI(b);
					break;
				}
				case 21:
				{
					GameScr.arrItemNonNu = new Item[msg.reader().readByte()];
					for (int num9 = 0; num9 < GameScr.arrItemNonNu.Length; num9++)
					{
						GameScr.arrItemNonNu[num9] = new Item();
						GameScr.arrItemNonNu[num9].typeUI = b;
						GameScr.arrItemNonNu[num9].indexUI = msg.reader().readUnsignedByte();
						GameScr.arrItemNonNu[num9].template = ItemTemplates.get(msg.reader().readShort());
					}
					CLock.HuyLockTypeUI(b);
					break;
				}
				case 22:
				{
					GameScr.arrItemAoNam = new Item[msg.reader().readByte()];
					for (int num25 = 0; num25 < GameScr.arrItemAoNam.Length; num25++)
					{
						GameScr.arrItemAoNam[num25] = new Item();
						GameScr.arrItemAoNam[num25].typeUI = b;
						GameScr.arrItemAoNam[num25].indexUI = msg.reader().readUnsignedByte();
						GameScr.arrItemAoNam[num25].template = ItemTemplates.get(msg.reader().readShort());
					}
					CLock.HuyLockTypeUI(b);
					break;
				}
				case 23:
				{
					GameScr.arrItemAoNu = new Item[msg.reader().readByte()];
					for (int num18 = 0; num18 < GameScr.arrItemAoNu.Length; num18++)
					{
						GameScr.arrItemAoNu[num18] = new Item();
						GameScr.arrItemAoNu[num18].typeUI = b;
						GameScr.arrItemAoNu[num18].indexUI = msg.reader().readUnsignedByte();
						GameScr.arrItemAoNu[num18].template = ItemTemplates.get(msg.reader().readShort());
					}
					CLock.HuyLockTypeUI(b);
					break;
				}
				case 24:
				{
					GameScr.arrItemGangTayNam = new Item[msg.reader().readByte()];
					for (int num12 = 0; num12 < GameScr.arrItemGangTayNam.Length; num12++)
					{
						GameScr.arrItemGangTayNam[num12] = new Item();
						GameScr.arrItemGangTayNam[num12].typeUI = b;
						GameScr.arrItemGangTayNam[num12].indexUI = msg.reader().readUnsignedByte();
						GameScr.arrItemGangTayNam[num12].template = ItemTemplates.get(msg.reader().readShort());
					}
					CLock.HuyLockTypeUI(b);
					break;
				}
				case 25:
				{
					GameScr.arrItemGangTayNu = new Item[msg.reader().readByte()];
					for (int num28 = 0; num28 < GameScr.arrItemGangTayNu.Length; num28++)
					{
						GameScr.arrItemGangTayNu[num28] = new Item();
						GameScr.arrItemGangTayNu[num28].typeUI = b;
						GameScr.arrItemGangTayNu[num28].indexUI = msg.reader().readUnsignedByte();
						GameScr.arrItemGangTayNu[num28].template = ItemTemplates.get(msg.reader().readShort());
					}
					CLock.HuyLockTypeUI(b);
					break;
				}
				case 26:
				{
					GameScr.arrItemQuanNam = new Item[msg.reader().readByte()];
					for (int num24 = 0; num24 < GameScr.arrItemQuanNam.Length; num24++)
					{
						GameScr.arrItemQuanNam[num24] = new Item();
						GameScr.arrItemQuanNam[num24].typeUI = b;
						GameScr.arrItemQuanNam[num24].indexUI = msg.reader().readUnsignedByte();
						GameScr.arrItemQuanNam[num24].template = ItemTemplates.get(msg.reader().readShort());
					}
					CLock.HuyLockTypeUI(b);
					break;
				}
				case 27:
				{
					GameScr.arrItemQuanNu = new Item[msg.reader().readByte()];
					for (int num19 = 0; num19 < GameScr.arrItemQuanNu.Length; num19++)
					{
						GameScr.arrItemQuanNu[num19] = new Item();
						GameScr.arrItemQuanNu[num19].typeUI = b;
						GameScr.arrItemQuanNu[num19].indexUI = msg.reader().readUnsignedByte();
						GameScr.arrItemQuanNu[num19].template = ItemTemplates.get(msg.reader().readShort());
					}
					CLock.HuyLockTypeUI(b);
					break;
				}
				case 28:
				{
					GameScr.arrItemGiayNam = new Item[msg.reader().readByte()];
					for (int num15 = 0; num15 < GameScr.arrItemGiayNam.Length; num15++)
					{
						GameScr.arrItemGiayNam[num15] = new Item();
						GameScr.arrItemGiayNam[num15].typeUI = b;
						GameScr.arrItemGiayNam[num15].indexUI = msg.reader().readUnsignedByte();
						GameScr.arrItemGiayNam[num15].template = ItemTemplates.get(msg.reader().readShort());
					}
					CLock.HuyLockTypeUI(b);
					break;
				}
				case 29:
				{
					GameScr.arrItemGiayNu = new Item[msg.reader().readByte()];
					for (int num10 = 0; num10 < GameScr.arrItemGiayNu.Length; num10++)
					{
						GameScr.arrItemGiayNu[num10] = new Item();
						GameScr.arrItemGiayNu[num10].typeUI = b;
						GameScr.arrItemGiayNu[num10].indexUI = msg.reader().readUnsignedByte();
						GameScr.arrItemGiayNu[num10].template = ItemTemplates.get(msg.reader().readShort());
					}
					CLock.HuyLockTypeUI(b);
					break;
				}
				case 16:
				{
					GameScr.arrItemLien = new Item[msg.reader().readByte()];
					for (int l = 0; l < GameScr.arrItemLien.Length; l++)
					{
						GameScr.arrItemLien[l] = new Item();
						GameScr.arrItemLien[l].typeUI = b;
						GameScr.arrItemLien[l].indexUI = msg.reader().readUnsignedByte();
						GameScr.arrItemLien[l].template = ItemTemplates.get(msg.reader().readShort());
					}
					CLock.HuyLockTypeUI(b);
					break;
				}
				case 17:
				{
					GameScr.arrItemNhan = new Item[msg.reader().readByte()];
					for (int num26 = 0; num26 < GameScr.arrItemNhan.Length; num26++)
					{
						GameScr.arrItemNhan[num26] = new Item();
						GameScr.arrItemNhan[num26].typeUI = b;
						GameScr.arrItemNhan[num26].indexUI = msg.reader().readUnsignedByte();
						GameScr.arrItemNhan[num26].template = ItemTemplates.get(msg.reader().readShort());
					}
					CLock.HuyLockTypeUI(b);
					break;
				}
				case 18:
				{
					GameScr.arrItemNgocBoi = new Item[msg.reader().readByte()];
					for (int num23 = 0; num23 < GameScr.arrItemNgocBoi.Length; num23++)
					{
						GameScr.arrItemNgocBoi[num23] = new Item();
						GameScr.arrItemNgocBoi[num23].typeUI = b;
						GameScr.arrItemNgocBoi[num23].indexUI = msg.reader().readUnsignedByte();
						GameScr.arrItemNgocBoi[num23].template = ItemTemplates.get(msg.reader().readShort());
					}
					CLock.HuyLockTypeUI(b);
					break;
				}
				case 19:
				{
					GameScr.arrItemPhu = new Item[msg.reader().readByte()];
					for (int num20 = 0; num20 < GameScr.arrItemPhu.Length; num20++)
					{
						GameScr.arrItemPhu[num20] = new Item();
						GameScr.arrItemPhu[num20].typeUI = b;
						GameScr.arrItemPhu[num20].indexUI = msg.reader().readUnsignedByte();
						GameScr.arrItemPhu[num20].template = ItemTemplates.get(msg.reader().readShort());
					}
					CLock.HuyLockTypeUI(b);
					break;
				}
				case 2:
				{
					GameScr.arrItemWeapon = new Item[msg.reader().readByte()];
					for (int num17 = 0; num17 < GameScr.arrItemWeapon.Length; num17++)
					{
						GameScr.arrItemWeapon[num17] = new Item();
						GameScr.arrItemWeapon[num17].typeUI = b;
						GameScr.arrItemWeapon[num17].indexUI = msg.reader().readUnsignedByte();
						GameScr.arrItemWeapon[num17].template = ItemTemplates.get(msg.reader().readShort());
					}
					CLock.HuyLockTypeUI(b);
					break;
				}
				case 6:
				{
					GameScr.arrItemStack = new Item[msg.reader().readByte()];
					for (int num14 = 0; num14 < GameScr.arrItemStack.Length; num14++)
					{
						GameScr.arrItemStack[num14] = new Item();
						GameScr.arrItemStack[num14].typeUI = b;
						GameScr.arrItemStack[num14].indexUI = msg.reader().readUnsignedByte();
						GameScr.arrItemStack[num14].template = ItemTemplates.get(msg.reader().readShort());
					}
					CLock.HuyLockTypeUI(b);
					break;
				}
				case 7:
				{
					GameScr.arrItemStackLock = new Item[msg.reader().readByte()];
					for (int num11 = 0; num11 < GameScr.arrItemStackLock.Length; num11++)
					{
						GameScr.arrItemStackLock[num11] = new Item();
						GameScr.arrItemStackLock[num11].typeUI = b;
						GameScr.arrItemStackLock[num11].isLock = true;
						GameScr.arrItemStackLock[num11].indexUI = msg.reader().readUnsignedByte();
						GameScr.arrItemStackLock[num11].template = ItemTemplates.get(msg.reader().readShort());
					}
					CLock.HuyLockTypeUI(b);
					break;
				}
				case 8:
				{
					GameScr.arrItemGrocery = new Item[msg.reader().readByte()];
					for (int n = 0; n < GameScr.arrItemGrocery.Length; n++)
					{
						GameScr.arrItemGrocery[n] = new Item();
						GameScr.arrItemGrocery[n].typeUI = b;
						GameScr.arrItemGrocery[n].indexUI = msg.reader().readUnsignedByte();
						GameScr.arrItemGrocery[n].template = ItemTemplates.get(msg.reader().readShort());
					}
					CLock.HuyLockTypeUI(b);
					break;
				}
				case 9:
				{
					GameScr.arrItemGroceryLock = new Item[msg.reader().readByte()];
					for (int k = 0; k < GameScr.arrItemGroceryLock.Length; k++)
					{
						GameScr.arrItemGroceryLock[k] = new Item();
						GameScr.arrItemGroceryLock[k].typeUI = b;
						GameScr.arrItemGroceryLock[k].isLock = true;
						GameScr.arrItemGroceryLock[k].indexUI = msg.reader().readUnsignedByte();
						GameScr.arrItemGroceryLock[k].template = ItemTemplates.get(msg.reader().readShort());
					}
					CLock.HuyLockTypeUI(b);
					break;
				}
				}
				break;
			}
			case 34:
			{
				MyVector myVector4 = new MyVector();
				string text17 = msg.reader().readUTF();
				if (!text17.Equals(string.Empty))
				{
					GameScr.gI().showAlert(null, text17, withMenuShow: true);
				}
				int num120 = msg.reader().readByte();
				for (int num121 = 0; num121 < num120; num121++)
				{
					string caption = msg.reader().readUTF();
					short num122 = msg.reader().readShort();
					myVector4.addElement(new Command(caption, GameCanvas.instance, 88819, num122));
				}
				GameCanvas.menu.startAt(myVector4, 3);
				break;
			}
			case 36:
				GameCanvas.debug("SA58", 2);
				GameScr.gI().openUIZone(msg);
				break;
			case 37:
				GameCanvas.debug("SA59", 2);
				GameScr.gI().tradeName = msg.reader().readUTF();
				GameScr.gI().openUITrade();
				break;
			case 38:
			{
				GameCanvas.debug("SA67", 2);
				int num118 = msg.reader().readShort();
				for (int num119 = 0; num119 < GameScr.vNpc.size(); num119++)
				{
					Npc npc2 = (Npc)GameScr.vNpc.elementAt(num119);
					if (npc2 != null && npc2.template.npcTemplateId == num118 && npc2.Equals(Char.getMyChar().npcFocus))
					{
						ChatPopup.addChatPopupMultiLine(msg.reader().readUTF(), 1000, npc2);
						break;
					}
				}
				break;
			}
			case 39:
			{
				GameCanvas.debug("SA68", 2);
				int num113 = msg.reader().readShort();
				for (int num114 = 0; num114 < GameScr.vNpc.size(); num114++)
				{
					Npc npc = (Npc)GameScr.vNpc.elementAt(num114);
					if (npc != null && npc.template.npcTemplateId == num113 && npc.Equals(Char.getMyChar().npcFocus))
					{
						ChatPopup.addChatPopup(msg.reader().readUTF(), 1000, npc);
						string[] array10 = new string[msg.reader().readByte()];
						for (int num115 = 0; num115 < array10.Length; num115++)
						{
							array10[num115] = msg.reader().readUTF();
						}
						GameScr.gI().createMenu(array10, npc);
						break;
					}
				}
				break;
			}
			case 40:
			{
				GameCanvas.debug("SA51", 2);
				InfoDlg.hide();
				GameCanvas.clearKeyHold();
				GameCanvas.clearKeyPressed();
				MyVector myVector3 = new MyVector();
				try
				{
					while (true)
					{
						myVector3.addElement(new Command(msg.reader().readUTF(), GameCanvas.instance, 88822, null));
					}
				}
				catch (Exception)
				{
				}
				if (Char.getMyChar().npcFocus == null)
				{
					return;
				}
				for (int num106 = 0; num106 < Char.getMyChar().npcFocus.template.menu.Length; num106++)
				{
					string[] array8 = Char.getMyChar().npcFocus.template.menu[num106];
					myVector3.addElement(new Command(array8[0], GameCanvas.instance, 88820, array8));
				}
				GameCanvas.menu.startAt(myVector3, 3);
				break;
			}
			case 42:
				GameCanvas.debug("SA57", 2);
				requestItemInfo(msg);
				break;
			case 43:
			{
				GameCanvas.debug("SA48", 2);
				int num105 = msg.reader().readInt();
				Char char28 = GameScr.findCharInMap(num105);
				if (char28 != null)
				{
					GameCanvas.startYesNoDlg(char28.cName + " " + mResources.INVITETRADE, 88810, num105, 88811, null);
				}
				break;
			}
			case 45:
			{
				GameCanvas.debug("SA50", 2);
				GameScr.gI().typeTradeOrder = 1;
				GameScr.gI().coinTradeOrder = msg.reader().readInt();
				GameScr.arrItemTradeOrder = new Item[12];
				int num93 = msg.reader().readByte();
				for (int num94 = 0; num94 < num93; num94++)
				{
					GameScr.arrItemTradeOrder[num94] = new Item();
					GameScr.arrItemTradeOrder[num94].typeUI = 3;
					GameScr.arrItemTradeOrder[num94].indexUI = num94;
					GameScr.arrItemTradeOrder[num94].template = ItemTemplates.get(msg.reader().readShort());
					GameScr.arrItemTradeOrder[num94].isLock = false;
					if (GameScr.arrItemTradeOrder[num94].isTypeBody() || GameScr.arrItemTradeOrder[num94].isTypeNgocKham())
					{
						GameScr.arrItemTradeOrder[num94].upgrade = msg.reader().readByte();
					}
					GameScr.arrItemTradeOrder[num94].isExpires = msg.reader().readBoolean();
					GameScr.arrItemTradeOrder[num94].quantity = msg.reader().readShort();
				}
				if (GameScr.gI().typeTrade == 1 && GameScr.gI().typeTradeOrder == 1)
				{
					GameScr.gI().timeTrade = (int)(mSystem.getCurrentTimeMillis() / 1000 + 5);
				}
				break;
			}
			case 46:
				GameCanvas.debug("SA49", 2);
				GameScr.gI().typeTradeOrder = 2;
				if (GameScr.gI().typeTrade >= 2 && GameScr.gI().typeTradeOrder >= 2)
				{
					InfoDlg.showWait();
				}
				break;
			case 47:
			{
				GameCanvas.debug("SA52", 2);
				GameCanvas.taskTick = 150;
				short taskId = msg.reader().readShort();
				sbyte index = msg.reader().readByte();
				string name = msg.reader().readUTF();
				string detail = msg.reader().readUTF();
				string[] array6 = new string[msg.reader().readByte()];
				short[] array7 = new short[array6.Length];
				short count = -1;
				for (int num79 = 0; num79 < array6.Length; num79++)
				{
					string text13 = msg.reader().readUTF();
					array7[num79] = -1;
					if (!text13.Equals(string.Empty))
					{
						array6[num79] = text13;
					}
				}
				try
				{
					count = msg.reader().readShort();
					for (int num80 = 0; num80 < array6.Length; num80++)
					{
						array7[num80] = msg.reader().readShort();
					}
				}
				catch (Exception)
				{
				}
				Char.getMyChar().taskMaint = new Task(taskId, index, name, detail, array6, array7, count);
				Char.getMyChar().callEffTask(21);
				if (Char.getMyChar().npcFocus != null)
				{
					Npc.clearEffTask();
				}
				else
				{
					AutoTask.StopLock();
				}
				break;
			}
			case 48:
				GameCanvas.debug("SA53", 2);
				GameCanvas.taskTick = 100;
				Char.getMyChar().taskMaint.index++;
				Char.getMyChar().taskMaint.count = 0;
				if (Char.getMyChar().npcFocus != null && Char.getMyChar().npcFocus.chatPopup != null && Char.getMyChar().taskMaint.index >= 2)
				{
					Char.getMyChar().npcFocus.chatPopup = null;
				}
				if (Char.getMyChar().taskMaint.index >= Char.getMyChar().taskMaint.subNames.Length - 1)
				{
					Char.getMyChar().callEffTask(61);
				}
				else
				{
					Char.getMyChar().callEffTask(21);
				}
				Npc.clearEffTask();
				AutoTask.StopLock();
				break;
			case 49:
				GameCanvas.debug("SA54", 2);
				Char.getMyChar().ctaskId++;
				Char.getMyChar().clearTask();
				AutoTask.StopLock();
				break;
			case 50:
				GameCanvas.taskTick = 50;
				GameCanvas.debug("SA55", 2);
				Char.getMyChar().taskMaint.count = msg.reader().readShort();
				if (Char.getMyChar().npcFocus != null)
				{
					Npc.clearEffTask();
				}
				break;
			case 51:
				mob = null;
				try
				{
					mob = Mob.get_Mob(msg.reader().readUnsignedByte());
				}
				catch (Exception)
				{
				}
				if (mob != null)
				{
					mob.hp = msg.reader().readInt();
					GameScr.startFlyText(string.Empty, mob.x, mob.y - mob.h, 0, -2, mFont.MISS);
				}
				break;
			case 52:
				GameCanvas.debug("SA5", 2);
				Char.ischangingMap = false;
				Char.isLockKey = false;
				Char.getMyChar().cx = msg.reader().readShort();
				Char.getMyChar().cy = msg.reader().readShort();
				Char.getMyChar().cxSend = Char.getMyChar().cx;
				Char.getMyChar().cySend = Char.getMyChar().cy;
				break;
			case 53:
			{
				GameCanvas.debug("SA4", 2);
				GameScr.gI().resetButton();
				string text9 = msg.reader().readUTF();
				if (!text9.Equals("typemoi"))
				{
					string str = msg.reader().readUTF();
					GameScr.gI().showAlert(text9, str, withMenuShow: false);
					break;
				}
				string title = msg.reader().readUTF();
				short time = msg.reader().readShort();
				string totalMoney = msg.reader().readUTF();
				short percentWin = msg.reader().readShort();
				string percentWin2 = msg.reader().readUTF();
				short numPlayer = msg.reader().readShort();
				string winnerInfo = msg.reader().readUTF();
				sbyte typeLucky = msg.reader().readByte();
				string myMoney = msg.reader().readUTF();
				GameScr.gI().showLucky_Draw(title, time, totalMoney, percentWin, percentWin2, numPlayer, winnerInfo, myMoney, typeLucky);
				break;
			}
			case 54:
				GameCanvas.debug("SA44", 2);
				GameCanvas.gI().openWeb(msg.reader().readUTF(), msg.reader().readUTF(), msg.reader().readUTF(), msg.reader().readUTF());
				break;
			case 55:
				GameCanvas.debug("SA444", 2);
				GameCanvas.gI().sendSms(msg.reader().readUTF(), msg.reader().readUTF(), msg.reader().readShort(), msg.reader().readUTF(), msg.reader().readUTF());
				break;
			case 57:
				GameCanvas.debug("SA44444", 2);
				GameCanvas.endDlg();
				GameScr.gI().resetButton();
				break;
			case 58:
				GameCanvas.debug("SA444444", 2);
				GameScr.arrItemTradeMe = null;
				GameScr.arrItemTradeOrder = null;
				if (GameScr.gI().coinTradeOrder > 0)
				{
					GameScr gameScr = GameScr.gI();
					string tradeItemName = gameScr.tradeItemName;
					gameScr.tradeItemName = tradeItemName + ", " + GameScr.gI().coinTradeOrder + " " + mResources.XU;
					GameScr.startFlyText("+" + GameScr.gI().coinTradeOrder, Char.getMyChar().cx, Char.getMyChar().cy - Char.getMyChar().ch - 10, 0, -2, mFont.ADDMONEY);
				}
				GameScr.gI().coinTrade = (GameScr.gI().coinTradeOrder = 0);
				GameScr.gI().resetButton();
				Char.getMyChar().xu = msg.reader().readInt();
				InfoDlg.hide();
				if (!GameScr.gI().tradeItemName.Equals(string.Empty))
				{
					InfoMe.addInfo(mResources.RECEIVE + " " + GameScr.gI().tradeItemName);
				}
				break;
			case 59:
			{
				GameCanvas.debug("SA48888", 2);
				string text6 = msg.reader().readUTF();
				Friend o = new Friend(text6, 4);
				GameScr.vFriendWait.addElement(o);
				InfoMe.addInfo(text6 + " " + mResources.FRIEND_ADDED, 20, mFont.tahoma_7_white);
				if (!GameScr.isPaintFriend)
				{
					break;
				}
				bool flag = false;
				for (int num34 = 0; num34 < GameScr.vFriend.size(); num34++)
				{
					if (((Friend)GameScr.vFriend.elementAt(num34)).friendName.Equals(text6))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					GameScr.vFriend.addElement(o);
					GameScr.gI().sortList(0);
					GameScr.indexRow = 0;
					GameScr.scrMain.clear();
				}
				break;
			}
			case 60:
			{
				GameCanvas.debug("SA769991", 2);
				Char char13 = GameScr.findCharInMap(msg.reader().readInt());
				if (char13 == null)
				{
					return;
				}
				if ((TileMap.tileTypeAtPixel(char13.cx, char13.cy) & TileMap.T_TOP) == TileMap.T_TOP)
				{
					sbyte b2 = msg.reader().readByte();
					char13.setSkillPaint(GameScr.sks[b2], 0);
					Mob.interestChar.myskill.template.id = b2;
				}
				else
				{
					sbyte b3 = msg.reader().readByte();
					char13.setSkillPaint(GameScr.sks[b3], 1);
					Mob.interestChar.myskill.template.id = b3;
				}
				GameCanvas.debug("SA769991v2", 2);
				if (char13.isWolf)
				{
					char13.isWolf = false;
					char13.timeSummon = mSystem.currentTimeMillis();
					if (char13.vitaWolf >= 500)
					{
						ServerEffect.addServerEffect(60, char13, 1);
					}
				}
				if (char13.isMoto)
				{
					char13.isMoto = false;
					char13.isMotoBehind = true;
					ServerEffect.addServerEffect(60, char13, 1);
				}
				Mob[] array = new Mob[10];
				int num31 = 0;
				try
				{
					for (num31 = 0; num31 < array.Length; num31++)
					{
						if (msg.reader().available() <= 0)
						{
							break;
						}
						int iD = msg.reader().readUnsignedByte();
						mob = (array[num31] = Mob.get_Mob(iD));
						if (num31 == 0)
						{
							if (char13.cx <= mob.x)
							{
								char13.cdir = 1;
							}
							else
							{
								char13.cdir = -1;
							}
						}
					}
				}
				catch (Exception)
				{
					Out.println("bi vang ra");
				}
				GameCanvas.debug("SA769992", 2);
				if (num31 > 0)
				{
					char13.attMobs = new Mob[num31];
					for (num31 = 0; num31 < char13.attMobs.Length; num31++)
					{
						char13.attMobs[num31] = array[num31];
					}
					char13.mobFocus = char13.attMobs[0];
				}
				break;
			}
			case 61:
			{
				GameCanvas.debug("SA7666", 2);
				Char char29 = GameScr.findCharInMap(msg.reader().readInt());
				if (char29 == null)
				{
					return;
				}
				if ((TileMap.tileTypeAtPixel(char29.cx, char29.cy) & TileMap.T_TOP) == TileMap.T_TOP)
				{
					sbyte b9 = msg.reader().readByte();
					char29.setSkillPaint(GameScr.sks[b9], 0);
				}
				else
				{
					sbyte b10 = msg.reader().readByte();
					char29.setSkillPaint(GameScr.sks[b10], 1);
				}
				if (char29.isWolf)
				{
					char29.isWolf = false;
					char29.timeSummon = mSystem.getCurrentTimeMillis();
					if (char29.vitaWolf >= 500)
					{
						ServerEffect.addServerEffect(60, char29, 1);
					}
				}
				if (char29.isMoto)
				{
					char29.isMoto = false;
					char29.isMotoBehind = true;
					ServerEffect.addServerEffect(60, char29, 1);
				}
				Char[] array9 = new Char[10];
				int num110 = 0;
				try
				{
					for (num110 = 0; num110 < array9.Length; num110++)
					{
						int num111 = msg.reader().readInt();
						Char char30 = (array9[num110] = ((num111 != Char.getMyChar().charID) ? GameScr.findCharInMap(num111) : Char.getMyChar()));
						if (num110 == 0)
						{
							if (char29.cx <= char30.cx)
							{
								char29.cdir = 1;
							}
							else
							{
								char29.cdir = -1;
							}
						}
					}
				}
				catch (Exception)
				{
				}
				GameCanvas.debug("SA7666x7", 2);
				if (num110 > 0)
				{
					char29.attChars = new Char[num110];
					for (num110 = 0; num110 < char29.attChars.Length; num110++)
					{
						char29.attChars[num110] = array9[num110];
					}
					char29.charFocus = char29.attChars[0];
				}
				break;
			}
			case 62:
			{
				GameCanvas.debug("SXX6", 2);
				int num88 = msg.reader().readInt();
				if (num88 == Char.getMyChar().charID)
				{
					Char myChar = Char.getMyChar();
					myChar.cHP = msg.reader().readInt();
					int num89 = msg.reader().readInt();
					int num90 = 0;
					try
					{
						myChar.cMP = msg.reader().readInt();
						num90 = msg.reader().readInt();
					}
					catch (Exception)
					{
					}
					num89 += num90;
					if (num89 == 0)
					{
						GameScr.startFlyText(string.Empty, myChar.cx, myChar.cy - myChar.ch, 0, -2, mFont.MISS_ME);
					}
					else if (num89 < 0)
					{
						GameScr.startFlyText("-" + num89 * -1, myChar.cx, myChar.cy - myChar.ch, 0, -2, mFont.FATAL_ME);
					}
					else
					{
						GameScr.startFlyText("-" + num89, myChar.cx, myChar.cy - myChar.ch, 0, -2, mFont.RED);
					}
					break;
				}
				Char char26 = GameScr.findCharInMap(num88);
				if (char26 == null)
				{
					return;
				}
				char26.cHP = msg.reader().readInt();
				int num91 = msg.reader().readInt();
				int num92 = 0;
				try
				{
					char26.cMP = msg.reader().readInt();
					num92 = msg.reader().readInt();
				}
				catch (Exception)
				{
				}
				num91 += num92;
				if (num91 == 0)
				{
					GameScr.startFlyText(string.Empty, char26.cx, char26.cy - char26.ch, 0, -2, mFont.MISS);
				}
				else if (num91 < 0)
				{
					GameScr.startFlyText("-" + num91 * -1, char26.cx, char26.cy - char26.ch, 0, -2, mFont.FATAL);
				}
				else
				{
					GameScr.startFlyText("-" + num91, char26.cx, char26.cy - char26.ch, 0, -2, mFont.ORANGE);
				}
				break;
			}
			case 63:
			{
				GameCanvas.debug("SZ6", 2);
				MyVector myVector2 = new MyVector();
				while (true)
				{
					try
					{
						myVector2.addElement(new Command(msg.reader().readUTF(), GameCanvas.instance, 88817, null));
					}
					catch (Exception)
					{
						break;
					}
				}
				GameCanvas.menu.startAt(myVector2, 3);
				break;
			}
			case 64:
			{
				GameCanvas.debug("SZ7", 2);
				int num84 = msg.reader().readInt();
				Char obj2 = ((num84 != Char.getMyChar().charID) ? GameScr.findCharInMap(num84) : Char.getMyChar());
				obj2.moveFast = new short[3];
				obj2.moveFast[0] = 0;
				short num85 = msg.reader().readShort();
				short num86 = msg.reader().readShort();
				obj2.moveFast[1] = num85;
				obj2.moveFast[2] = num86;
				try
				{
					num84 = msg.reader().readInt();
					Char obj3 = ((num84 != Char.getMyChar().charID) ? GameScr.findCharInMap(num84) : Char.getMyChar());
					obj3.cx = num85;
					obj3.cy = num86;
				}
				catch (Exception)
				{
					Out.println(" loi tai cmd   " + msg.command);
				}
				break;
			}
			case 66:
			{
				GameCanvas.debug("SZ1", 2);
				int num71 = msg.reader().readInt();
				int num72 = msg.reader().readInt();
				if (num71 != Char.getMyChar().charID && num72 != Char.getMyChar().charID)
				{
					GameScr.findCharInMap(num71).testCharId = num72;
					GameScr.findCharInMap(num72).testCharId = num71;
				}
				else if (num71 == Char.getMyChar().charID)
				{
					Char.getMyChar().testCharId = num72;
					Char.getMyChar().npcFocus = null;
					Char.getMyChar().mobFocus = null;
					Char.getMyChar().itemFocus = null;
					Char.getMyChar().charFocus = GameScr.findCharInMap(Char.getMyChar().testCharId);
					Char.getMyChar().charFocus.testCharId = Char.getMyChar().charID;
					GameScr.gI().cPreFocusID = GameScr.gI().cLastFocusID;
					GameScr.gI().cLastFocusID = num72;
					Char.isManualFocus = true;
				}
				else if (num72 == Char.getMyChar().charID)
				{
					Char.getMyChar().testCharId = num71;
					Char.getMyChar().npcFocus = null;
					Char.getMyChar().mobFocus = null;
					Char.getMyChar().itemFocus = null;
					Char.getMyChar().charFocus = GameScr.findCharInMap(Char.getMyChar().testCharId);
					Char.getMyChar().charFocus.testCharId = Char.getMyChar().charID;
					GameScr.gI().cPreFocusID = GameScr.gI().cLastFocusID;
					GameScr.gI().cLastFocusID = num71;
					Char.isManualFocus = true;
				}
				break;
			}
			case 67:
			{
				GameCanvas.debug("SZ2", 2);
				int num = msg.reader().readInt();
				int num2 = msg.reader().readInt();
				int num3 = 0;
				try
				{
					num3 = msg.reader().readInt();
				}
				catch (Exception)
				{
				}
				if (num == Char.getMyChar().charID)
				{
					Char char2 = GameScr.findCharInMap(num2);
					if (num3 > 0)
					{
						InfoMe.addInfo(mResources.replace(mResources.YOU_LOST, char2.cName));
						Char.getMyChar().cHP = num3;
						Char.getMyChar().resultTest = 29;
						if (char2 != null)
						{
							char2.resultTest = 89;
						}
					}
					else
					{
						if (char2 != null)
						{
							char2.resultTest = 59;
						}
						Char.getMyChar().resultTest = 59;
						InfoMe.addInfo(mResources.replace(mResources.TEST_END, char2.cName));
					}
					Char.getMyChar().testCharId = -9999;
					Char.getMyChar().charFocus = null;
					if (GameScr.gI().cPreFocusID >= 0)
					{
						GameScr.gI().cLastFocusID = GameScr.gI().cPreFocusID;
						GameScr.gI().cPreFocusID = -1;
					}
					else
					{
						GameScr.gI().cLastFocusID = -1;
					}
					if (char2 != null)
					{
						char2.testCharId = -9999;
					}
					break;
				}
				if (num2 == Char.getMyChar().charID)
				{
					Char char3 = GameScr.findCharInMap(num);
					if (num3 > 0)
					{
						if (char3 != null)
						{
							char3.cHP = num3;
						}
						if (char3 != null)
						{
							char3.resultTest = 29;
						}
						Char.getMyChar().resultTest = 89;
						InfoMe.addInfo(mResources.replace(mResources.YOU_WIN, char3.cName));
					}
					else
					{
						if (char3 != null)
						{
							char3.resultTest = 59;
						}
						Char.getMyChar().resultTest = 59;
						InfoMe.addInfo(mResources.replace(mResources.TEST_END, char3.cName));
					}
					if (char3 != null)
					{
						char3.testCharId = -9999;
					}
					Char.getMyChar().testCharId = -9999;
					Char.getMyChar().charFocus = null;
					if (GameScr.gI().cPreFocusID >= 0)
					{
						GameScr.gI().cLastFocusID = GameScr.gI().cPreFocusID;
						GameScr.gI().cPreFocusID = -1;
					}
					else
					{
						GameScr.gI().cLastFocusID = -1;
					}
					break;
				}
				Char char4 = GameScr.findCharInMap(num);
				Char char5 = GameScr.findCharInMap(num2);
				if (num3 > 0)
				{
					if (char4 != null)
					{
						char4.cHP = num3;
					}
					if (char4 != null)
					{
						char4.resultTest = 29;
					}
					if (char5 != null)
					{
						char5.resultTest = 89;
					}
				}
				else
				{
					if (char4 != null)
					{
						char4.resultTest = 59;
					}
					if (char5 != null)
					{
						char5.resultTest = 59;
					}
				}
				if (char4 != null)
				{
					char4.testCharId = -9999;
				}
				if (char5 != null)
				{
					char5.testCharId = -9999;
				}
				break;
			}
			case 68:
			{
				GameCanvas.debug("SZ3", 2);
				Char @char = GameScr.findCharInMap(msg.reader().readInt());
				if (@char != null)
				{
					@char.killCharId = Char.getMyChar().charID;
					Char.getMyChar().npcFocus = null;
					Char.getMyChar().mobFocus = null;
					Char.getMyChar().itemFocus = null;
					Char.getMyChar().charFocus = @char;
					Char.isManualFocus = true;
					InfoMe.addInfo(@char.cName + mResources.CUU_SAT, 20, mFont.tahoma_7_red);
				}
				break;
			}
			case 69:
				GameCanvas.debug("SZ4", 2);
				Char.getMyChar().killCharId = msg.reader().readInt();
				Char.getMyChar().npcFocus = null;
				Char.getMyChar().mobFocus = null;
				Char.getMyChar().itemFocus = null;
				Char.getMyChar().charFocus = GameScr.findCharInMap(Char.getMyChar().killCharId);
				Char.isManualFocus = true;
				break;
			case 70:
			{
				GameCanvas.debug("SZ5", 2);
				Char char31 = Char.getMyChar();
				try
				{
					char31 = GameScr.findCharInMap(msg.reader().readInt());
				}
				catch (Exception)
				{
				}
				char31.killCharId = -9999;
				break;
			}
			case 71:
			{
				long num123 = msg.reader().readLong();
				Char.getMyChar().cExpDown -= num123;
				GameScr.startFlyText("+" + num123, Char.getMyChar().cx, Char.getMyChar().cy - Char.getMyChar().ch, 0, -2, mFont.GREEN);
				break;
			}
			case 72:
				GameCanvas.debug("SA88", 2);
				Char.getMyChar().cPk = msg.reader().readByte();
				Char.getMyChar().waitToDie(msg.reader().readShort(), msg.reader().readShort());
				Char.getMyChar().cEXP = GameScr.getMaxExp(Char.getMyChar().clevel - 1);
				Char.getMyChar().cExpDown = msg.reader().readLong();
				GameScr.setLevel_Exp(Char.getMyChar().cEXP, value: true);
				break;
			case 75:
			{
				GameCanvas.debug("SA6333e55", 2);
				BuNhin buNhin3 = new BuNhin(msg.reader().readUTF(), msg.reader().readShort(), msg.reader().readShort());
				GameScr.vBuNhin.addElement(buNhin3);
				ServerEffect.addServerEffect(60, buNhin3.x, buNhin3.y, 1);
				break;
			}
			case 76:
			{
				GameCanvas.debug("SA6333e155", 2);
				Mob mob8 = Mob.get_Mob(msg.reader().readUnsignedByte());
				if (mob8 != null)
				{
					BuNhin buNhin2 = GameScr.findBuNhinInMap(msg.reader().readShort());
					if (buNhin2 == null)
					{
						return;
					}
					short idSkill_atk4 = msg.reader().readShort();
					sbyte typeAtk4 = msg.reader().readByte();
					sbyte typeTool4 = msg.reader().readByte();
					mob8.setAttack(buNhin2);
					mob8.setTypeAtk(idSkill_atk4, typeAtk4, typeTool4);
				}
				break;
			}
			case 77:
			{
				GameCanvas.debug("SA6333e255", 2);
				BuNhin buNhin = (BuNhin)GameScr.vBuNhin.elementAt(msg.reader().readShort());
				if (buNhin != null)
				{
					GameScr.vBuNhin.removeElement(buNhin);
					ServerEffect.addServerEffect(60, buNhin.x, buNhin.y, 1);
				}
				break;
			}
			case 78:
				GameCanvas.debug("SA85", 2);
				mob = null;
				try
				{
					mob = Mob.get_Mob(msg.reader().readUnsignedByte());
				}
				catch (Exception)
				{
				}
				if (mob != null && mob.status != 0 && mob.status != 0)
				{
					mob.status = 0;
					ServerEffect.addServerEffect(60, mob.x, mob.y, 1);
					ItemMap itemMap6 = new ItemMap(msg.reader().readShort(), msg.reader().readShort(), mob.x, mob.y, msg.reader().readShort(), msg.reader().readShort());
					GameScr.vItemMap.addElement(itemMap6);
					if (Res.abs(itemMap6.y - Char.getMyChar().cy) < 24 && Res.abs(itemMap6.x - Char.getMyChar().cx) < 24)
					{
						Char.getMyChar().charFocus = null;
					}
				}
				break;
			case 79:
			{
				GameCanvas.debug("SA4888888", 2);
				int num112 = msg.reader().readInt();
				GameCanvas.startYesNoDlg(msg.reader().readUTF() + " " + mResources.INVITEPARTY, 8887, num112, 8888, num112);
				break;
			}
			case 82:
			{
				GameCanvas.debug("SXX1", 2);
				GameScr.vParty.removeAllElements();
				bool isLock = msg.reader().readBoolean();
				try
				{
					for (int num109 = 0; num109 < 6; num109++)
					{
						GameScr.vParty.addElement(new Party(msg.reader().readInt(), msg.reader().readByte(), msg.reader().readUTF(), isLock));
					}
				}
				catch (Exception)
				{
				}
				GameScr.gI().refreshTeam();
				break;
			}
			case 83:
				GameCanvas.debug("SXX2", 2);
				GameScr.vParty.removeAllElements();
				GameScr.gI().refreshTeam();
				break;
			case 84:
			{
				GameCanvas.debug("SXX3", 2);
				Friend friend = new Friend(msg.reader().readUTF(), msg.reader().readByte());
				GameScr.gI().actRemoveWaitAcceptFriend(friend.friendName);
				if (friend.type == 0)
				{
					InfoMe.addInfo(mResources.YOU_ADD + " " + friend.friendName + " " + mResources.TO_LIST);
					GameScr.vFriend.addElement(friend);
				}
				else if (friend.type == 1)
				{
					for (int num104 = 0; num104 < GameScr.vFriend.size(); num104++)
					{
						if (((Friend)GameScr.vFriend.elementAt(num104)).friendName.Equals(friend.friendName))
						{
							GameScr.vFriend.removeElementAt(num104);
							break;
						}
					}
					InfoMe.addInfo(mResources.YOU_AND + " " + friend.friendName + " " + mResources.BE_FRIEND);
					friend.type = 3;
					GameScr.vFriend.insertElementAt(friend, 0);
				}
				if (GameScr.isPaintFriend)
				{
					GameScr.gI().sortList(0);
					GameScr.indexRow = 0;
					GameScr.scrMain.clear();
				}
				break;
			}
			case 85:
			{
				GameCanvas.debug("SXX4", 2);
				Mob mob7 = Mob.get_Mob(msg.reader().readUnsignedByte());
				bool isDisable = msg.reader().readBoolean();
				if (mob7 != null)
				{
					mob7.isDisable = isDisable;
				}
				break;
			}
			case 86:
			{
				GameCanvas.debug("SXX5", 2);
				Mob mob6 = Mob.get_Mob(msg.reader().readUnsignedByte());
				bool isDontMove = msg.reader().readBoolean();
				if (mob6 != null)
				{
					mob6.isDontMove = isDontMove;
				}
				break;
			}
			case 87:
			{
				GameCanvas.debug("SXX8", 2);
				int num87 = msg.reader().readInt();
				Char char25 = ((num87 != Char.getMyChar().charID) ? GameScr.findCharInMap(num87) : Char.getMyChar());
				if (char25 == null)
				{
					return;
				}
				int iD2 = msg.reader().readUnsignedByte();
				short idSkill_atk2 = msg.reader().readShort();
				sbyte typeAtk2 = msg.reader().readByte();
				sbyte typeTool2 = msg.reader().readByte();
				sbyte b8 = 0;
				int charId3 = -1;
				try
				{
					b8 = msg.reader().readByte();
					if (b8 == 1)
					{
						charId3 = msg.reader().readInt();
					}
				}
				catch (Exception)
				{
				}
				if (char25.mobMe != null)
				{
					if (b8 == 0)
					{
						Mob mobToAttack = Mob.get_Mob(iD2);
						char25.mobMe.attackOtherMob(mobToAttack);
					}
					else
					{
						Char charToAttack = GameScr.findCharInMap(charId3);
						char25.mobMe.attackOtherChar(charToAttack);
					}
				}
				char25.mobMe.setTypeAtk(idSkill_atk2, typeAtk2, typeTool2);
				break;
			}
			case 88:
			{
				int num82 = msg.reader().readInt();
				Char char24;
				if (num82 == Char.getMyChar().charID)
				{
					char24 = Char.getMyChar();
				}
				else
				{
					char24 = GameScr.findCharInMap(num82);
					if (char24 == null)
					{
						return;
					}
				}
				char24.cHP = char24.cMaxHP;
				char24.cMP = char24.cMaxMP;
				char24.cx = msg.reader().readShort();
				char24.cy = msg.reader().readShort();
				char24.liveFromDead();
				break;
			}
			case 89:
			{
				GameCanvas.debug("SXX5", 2);
				Mob mob5 = Mob.get_Mob(msg.reader().readUnsignedByte());
				bool isFire = msg.reader().readBoolean();
				if (mob5 != null)
				{
					mob5.isFire = isFire;
				}
				break;
			}
			case 90:
			{
				GameCanvas.debug("SXX5", 2);
				Mob mob4 = Mob.get_Mob(msg.reader().readUnsignedByte());
				bool isIce = msg.reader().readBoolean();
				if (mob4 != null)
				{
					mob4.isIce = isIce;
					if (!mob4.isIce)
					{
						ServerEffect.addServerEffect(77, mob4.x, mob4.y - 9, 1);
					}
				}
				break;
			}
			case 91:
			{
				GameCanvas.debug("SXX5", 2);
				Mob mob3 = Mob.get_Mob(msg.reader().readUnsignedByte());
				bool isWind = msg.reader().readBoolean();
				if (mob3 != null)
				{
					mob3.isWind = isWind;
				}
				break;
			}
			case 92:
			{
				string info = msg.reader().readUTF();
				short num77 = msg.reader().readShort();
				GameCanvas.inputDlg.show(info, new Command(mResources.ACCEPT, GameCanvas.instance, 88818, num77), TField.INPUT_TYPE_ANY);
				break;
			}
			case 93:
			{
				int num63 = msg.reader().readInt();
				GameScr.currentCharViewInfo = new Char();
				if (Char.getMyChar().charID == num63)
				{
					GameScr.currentCharViewInfo = Char.getMyChar();
				}
				else
				{
					Char char20 = GameScr.findCharInMap(num63);
					if (char20 == null)
					{
						GameScr.currentCharViewInfo = new Char();
					}
					else
					{
						GameScr.currentCharViewInfo = char20;
					}
					GameScr.currentCharViewInfo.charID = num63;
					GameScr.currentCharViewInfo.statusMe = 1;
					GameScr.gI().showViewInfo();
				}
				GameScr.currentCharViewInfo.cName = msg.reader().readUTF();
				GameScr.currentCharViewInfo.head = msg.reader().readShort();
				GameScr.currentCharViewInfo.cgender = msg.reader().readByte();
				int num64 = msg.reader().readByte();
				GameScr.currentCharViewInfo.nClass = GameScr.nClasss[num64];
				GameScr.currentCharViewInfo.cPk = msg.reader().readByte();
				GameScr.currentCharViewInfo.cHP = msg.reader().readInt();
				GameScr.currentCharViewInfo.cMaxHP = msg.reader().readInt();
				GameScr.currentCharViewInfo.cMP = msg.reader().readInt();
				GameScr.currentCharViewInfo.cMaxMP = msg.reader().readInt();
				GameScr.currentCharViewInfo.cspeed = msg.reader().readByte();
				GameScr.currentCharViewInfo.cResFire = msg.reader().readShort();
				GameScr.currentCharViewInfo.cResIce = msg.reader().readShort();
				GameScr.currentCharViewInfo.cResWind = msg.reader().readShort();
				GameScr.currentCharViewInfo.cdame = msg.reader().readInt();
				GameScr.currentCharViewInfo.cdameDown = msg.reader().readInt();
				GameScr.currentCharViewInfo.cExactly = msg.reader().readShort();
				GameScr.currentCharViewInfo.cMiss = msg.reader().readShort();
				GameScr.currentCharViewInfo.cFatal = msg.reader().readShort();
				GameScr.currentCharViewInfo.cReactDame = msg.reader().readShort();
				GameScr.currentCharViewInfo.sysUp = msg.reader().readShort();
				GameScr.currentCharViewInfo.sysDown = msg.reader().readShort();
				GameScr.currentCharViewInfo.clevel = msg.reader().readUnsignedByte();
				GameScr.currentCharViewInfo.pointUydanh = msg.reader().readShort();
				GameScr.currentCharViewInfo.cClanName = msg.reader().readUTF();
				if (!GameScr.currentCharViewInfo.cClanName.Equals(string.Empty))
				{
					GameScr.currentCharViewInfo.ctypeClan = msg.reader().readByte();
				}
				GameScr.currentCharViewInfo.pointUydanh = msg.reader().readShort();
				GameScr.currentCharViewInfo.pointNon = msg.reader().readShort();
				GameScr.currentCharViewInfo.pointAo = msg.reader().readShort();
				GameScr.currentCharViewInfo.pointGangtay = msg.reader().readShort();
				GameScr.currentCharViewInfo.pointQuan = msg.reader().readShort();
				GameScr.currentCharViewInfo.pointGiay = msg.reader().readShort();
				GameScr.currentCharViewInfo.pointVukhi = msg.reader().readShort();
				GameScr.currentCharViewInfo.pointLien = msg.reader().readShort();
				GameScr.currentCharViewInfo.pointNhan = msg.reader().readShort();
				GameScr.currentCharViewInfo.pointNgocboi = msg.reader().readShort();
				GameScr.currentCharViewInfo.pointPhu = msg.reader().readShort();
				GameScr.currentCharViewInfo.countFinishDay = msg.reader().readByte();
				GameScr.currentCharViewInfo.countLoopBoos = msg.reader().readByte();
				GameScr.currentCharViewInfo.countPB = msg.reader().readByte();
				GameScr.currentCharViewInfo.limitTiemnangso = msg.reader().readByte();
				GameScr.currentCharViewInfo.limitKynangso = msg.reader().readByte();
				GameScr.currentCharViewInfo.arrItemBody = new Item[32];
				try
				{
					GameScr.currentCharViewInfo.setDefaultPart();
					for (int num65 = 0; num65 < 16; num65++)
					{
						short num66 = msg.reader().readShort();
						if (num66 > -1)
						{
							ItemTemplate itemTemplate = ItemTemplates.get(num66);
							int type = itemTemplate.type;
							GameScr.currentCharViewInfo.arrItemBody[type] = new Item();
							GameScr.currentCharViewInfo.arrItemBody[type].indexUI = type;
							GameScr.currentCharViewInfo.arrItemBody[type].typeUI = 5;
							GameScr.currentCharViewInfo.arrItemBody[type].template = itemTemplate;
							GameScr.currentCharViewInfo.arrItemBody[type].isLock = true;
							GameScr.currentCharViewInfo.arrItemBody[type].upgrade = msg.reader().readByte();
							GameScr.currentCharViewInfo.arrItemBody[type].sys = msg.reader().readByte();
							switch (type)
							{
							case 6:
								GameScr.currentCharViewInfo.leg = GameScr.currentCharViewInfo.arrItemBody[type].template.part;
								break;
							case 2:
								GameScr.currentCharViewInfo.body = GameScr.currentCharViewInfo.arrItemBody[type].template.part;
								break;
							case 1:
								GameScr.currentCharViewInfo.wp = GameScr.currentCharViewInfo.arrItemBody[type].template.part;
								break;
							}
						}
					}
				}
				catch (Exception)
				{
				}
				try
				{
					for (int num67 = 0; num67 < 16; num67++)
					{
						short num68 = msg.reader().readShort();
						if (num68 > -1)
						{
							ItemTemplate itemTemplate2 = ItemTemplates.get(num68);
							int num69 = itemTemplate2.type + 16;
							GameScr.currentCharViewInfo.arrItemBody[num69] = new Item();
							GameScr.currentCharViewInfo.arrItemBody[num69].indexUI = num69;
							GameScr.currentCharViewInfo.arrItemBody[num69].typeUI = 5;
							GameScr.currentCharViewInfo.arrItemBody[num69].template = itemTemplate2;
							GameScr.currentCharViewInfo.arrItemBody[num69].isLock = true;
							GameScr.currentCharViewInfo.arrItemBody[num69].upgrade = msg.reader().readByte();
							GameScr.currentCharViewInfo.arrItemBody[num69].sys = msg.reader().readByte();
							switch (num69)
							{
							case 6:
								GameScr.currentCharViewInfo.leg = GameScr.currentCharViewInfo.arrItemBody[num69].template.part;
								break;
							case 2:
								GameScr.currentCharViewInfo.body = GameScr.currentCharViewInfo.arrItemBody[num69].template.part;
								break;
							case 1:
								GameScr.currentCharViewInfo.wp = GameScr.currentCharViewInfo.arrItemBody[num69].template.part;
								break;
							}
						}
					}
				}
				catch (Exception)
				{
				}
				break;
			}
			case 95:
			{
				GameCanvas.debug("SA77", 22);
				int num59 = msg.reader().readInt();
				Char.getMyChar().xu += num59;
				GameScr.startFlyText((num59 <= 0) ? (string.Empty + num59) : ("+" + num59), Char.getMyChar().cx, Char.getMyChar().cy - Char.getMyChar().ch - 10, 0, -2, mFont.YELLOW);
				break;
			}
			case 96:
				GameCanvas.debug("SA77a", 22);
				Char.getMyChar().taskOrders.addElement(new TaskOrder(msg.reader().readByte(), msg.reader().readInt(), msg.reader().readInt(), msg.reader().readUTF(), msg.reader().readUTF(), msg.reader().readUnsignedByte(), msg.reader().readUnsignedByte()));
				Char.getMyChar().callEffTask(21);
				break;
			case 97:
			{
				sbyte b7 = msg.reader().readByte();
				for (int num58 = 0; num58 < Char.getMyChar().taskOrders.size(); num58++)
				{
					TaskOrder taskOrder2 = (TaskOrder)Char.getMyChar().taskOrders.elementAt(num58);
					if (taskOrder2 != null && taskOrder2.taskId == b7)
					{
						taskOrder2.count = msg.reader().readInt();
						if (taskOrder2.count == taskOrder2.maxCount)
						{
							Char.getMyChar().callEffTask(61);
						}
						break;
					}
				}
				break;
			}
			case 98:
			{
				sbyte b6 = msg.reader().readByte();
				for (int num57 = 0; num57 < Char.getMyChar().taskOrders.size(); num57++)
				{
					TaskOrder taskOrder = (TaskOrder)Char.getMyChar().taskOrders.elementAt(num57);
					if (taskOrder != null && taskOrder.taskId == b6)
					{
						Char.getMyChar().taskOrders.removeElementAt(num57);
						break;
					}
				}
				Char.getMyChar().callEffTask(21);
				break;
			}
			case 99:
			{
				Out.println("Vao DUN >>>>>");
				GameCanvas.debug("SA48", 2);
				Char char18 = GameScr.findCharInMap(msg.reader().readInt());
				if (char18 != null)
				{
					GameCanvas.startYesNoDlg(char18.cName + " " + mResources.INVITETESTDUN, 88840, char18, 8882, null);
				}
				break;
			}
			case 100:
			{
				GameScr.vList.removeAllElements();
				int num54 = msg.reader().readByte();
				for (int num55 = 0; num55 < num54; num55++)
				{
					try
					{
						DunItem dunItem = new DunItem();
						dunItem.id = msg.reader().readByte();
						dunItem.name1 = msg.reader().readUTF();
						dunItem.name2 = msg.reader().readUTF();
						GameScr.vList.addElement(dunItem);
					}
					catch (Exception)
					{
					}
				}
				GameScr.gI().doShowListUI();
				break;
			}
			case 101:
				try
				{
					GameScr.currentCharViewInfo.pointTinhTu = msg.reader().readInt();
					GameScr.currentCharViewInfo.limitPhongLoi = msg.reader().readByte();
					GameScr.currentCharViewInfo.limitBangHoa = msg.reader().readByte();
				}
				catch (Exception)
				{
				}
				break;
			case 103:
			{
				GameScr.indexMenu = msg.reader().readByte();
				GameScr.arrItemStands = new ItemStands[msg.reader().readInt()];
				for (int num53 = 0; num53 < GameScr.arrItemStands.Length; num53++)
				{
					GameScr.arrItemStands[num53] = new ItemStands();
					GameScr.arrItemStands[num53].item = new Item();
					GameScr.arrItemStands[num53].item.itemId = msg.reader().readInt();
					GameScr.arrItemStands[num53].timeStart = (int)(mSystem.getCurrentTimeMillis() / 1000);
					GameScr.arrItemStands[num53].timeEnd = msg.reader().readInt();
					GameScr.arrItemStands[num53].item.quantity = msg.reader().readUnsignedShort();
					GameScr.arrItemStands[num53].seller = msg.reader().readUTF();
					GameScr.arrItemStands[num53].price = msg.reader().readInt();
					GameScr.arrItemStands[num53].item.template = ItemTemplates.get(msg.reader().readShort());
				}
				GameScr.gI().doOpenUI(37);
				break;
			}
			case 104:
				viewItemAuction(msg);
				break;
			case 106:
			{
				GameCanvas.debug("SA48", 2);
				Char char17 = GameScr.findCharInMap(msg.reader().readInt());
				if (char17 != null)
				{
					GameCanvas.startYesNoDlg(char17.cName + " " + mResources.INVITETESTGT, 88841, char17, 8882, null);
				}
				break;
			}
			case 107:
			{
				int num52 = msg.reader().readByte();
				GameCanvas.startYesNoDlg(msg.reader().readUTF(), 8890, num52, 8882, null);
				break;
			}
			case 108:
				Char.getMyChar().itemMonToBag(msg);
				break;
			case 109:
			{
				GameCanvas.debug("SA51", 2);
				InfoDlg.hide();
				GameCanvas.clearKeyHold();
				GameCanvas.clearKeyPressed();
				MyVector myVector = new MyVector();
				try
				{
					int num49 = msg.reader().readByte();
					for (int num50 = 0; num50 < num49; num50++)
					{
						string[] array3 = new string[msg.reader().readByte()];
						for (int num51 = 0; num51 < array3.Length; num51++)
						{
							array3[num51] = msg.reader().readUTF();
						}
						myVector.addElement(new Command(array3[0], GameCanvas.instance, 88820, array3));
					}
				}
				catch (Exception)
				{
				}
				if (Char.getMyChar().npcFocus == null)
				{
					return;
				}
				GameCanvas.menu.startAt(myVector, 3);
				break;
			}
			case 112:
			{
				Item obj = Char.getMyChar().arrItemBag[msg.reader().readByte()];
				obj.upgrade = msg.reader().readByte();
				obj.expires = 0L;
				break;
			}
			case 114:
				GameScr.gI().typeba = msg.reader().readSByte();
				break;
			case 116:
			{
				Char char16 = GameScr.findCharInMap(msg.reader().readInt());
				if (char16 != null)
				{
					readCharInfo(char16, msg);
				}
				break;
			}
			case 117:
				try
				{
					Mob.vEggMonter.removeAllElements();
					TileMap.itemMap.clear();
					GameScr.vItemTreeBehind.removeAllElements();
					GameScr.vItemTreeBetwen.removeAllElements();
					GameScr.vItemTreeFront.removeAllElements();
					sbyte b5 = msg.reader().readsbyte();
					for (int num43 = 0; num43 < b5; num43++)
					{
						string k2 = msg.reader().readShort() + string.Empty;
						sbyte[] array2 = new sbyte[msg.reader().readInt()];
						msg.reader().readz(array2);
						object v = createImage(array2);
						TileMap.itemMap.put(k2, v);
					}
					int num44 = msg.reader().readUnsignedByte();
					for (int num45 = 0; num45 < num44; num45++)
					{
						int idTree = msg.reader().readUnsignedByte();
						byte x = msg.reader().readUnsignedByte();
						int y = msg.reader().readUnsignedByte();
						ItemTree itemTree = new ItemTree(x, y);
						itemTree.idTree = idTree;
						GameScr.vItemTreeBehind.addElement(itemTree);
					}
					num44 = msg.reader().readUnsignedByte();
					for (int num46 = 0; num46 < num44; num46++)
					{
						int idTree2 = msg.reader().readUnsignedByte();
						byte x2 = msg.reader().readUnsignedByte();
						int y2 = msg.reader().readUnsignedByte();
						ItemTree itemTree2 = new ItemTree(x2, y2);
						itemTree2.idTree = idTree2;
						GameScr.vItemTreeBetwen.addElement(itemTree2);
					}
					num44 = msg.reader().readUnsignedByte();
					for (int num47 = 0; num47 < num44; num47++)
					{
						int idTree3 = msg.reader().readUnsignedByte();
						byte x3 = msg.reader().readUnsignedByte();
						int y3 = msg.reader().readUnsignedByte();
						ItemTree itemTree3 = new ItemTree(x3, y3);
						itemTree3.idTree = idTree3;
						GameScr.vItemTreeFront.addElement(itemTree3);
					}
				}
				catch (Exception)
				{
				}
				break;
			case 119:
				switch (msg.reader().readByte())
				{
				case -1:
					GameScr.isUseitemAuto = true;
					GameScr.rangeSearch = msg.reader().readInt();
					if (GameScr.rangeSearch > 360)
					{
						GameScr.isAllmap = true;
						break;
					}
					GameScr.isAllmap = false;
					GameScr.pointCenterX = Char.getMyChar().cx;
					GameScr.pointCenterY = Char.getMyChar().cy;
					break;
				default:
					GameScr.isUseitemAuto = false;
					GameScr.auto = 0;
					break;
				case 0:
				{
					Char char15 = GameScr.findCharInMap(msg.reader().readInt());
					if (char15 != null)
					{
						ServerEffect.addServerEffect(141, char15.cx, char15.cy, 2);
						char15.cxMoveLast = msg.reader().readShort();
						char15.cyMoveLast = msg.reader().readShort();
						ServerEffect.addServerEffect(141, char15.cx, char15.cy, 2);
					}
					break;
				}
				}
				break;
			case 121:
			{
				GameScr.vList.removeAllElements();
				int num39 = msg.reader().readUnsignedByte();
				for (int num40 = 0; num40 < num39; num40++)
				{
					try
					{
						Ranked ranked = new Ranked();
						ranked.name = msg.reader().readUTF();
						ranked.ranked = msg.reader().readInt();
						ranked.stt = msg.reader().readUTF();
						GameScr.vList.addElement(ranked);
					}
					catch (Exception)
					{
					}
				}
				GameScr.gI().doShowRankedListUI();
				break;
			}
			case 122:
				switch (msg.reader().readByte())
				{
				case 0:
					addMob(msg);
					break;
				case 1:
					addEffAuto(msg);
					break;
				case 2:
					getImgEffAuto(msg);
					break;
				case 3:
					getDataEffAuto(msg);
					break;
				}
				break;
			case 123:
				switch (msg.reader().readByte())
				{
				case 0:
					GameCanvas.isKiemduyet_info = true;
					break;
				case 1:
					GameCanvas.isKiemduyet_info = false;
					break;
				case 2:
					RMS.saveRMSInt("isKiemduyet", 0);
					GameCanvas.isKiemduyet = true;
					break;
				default:
					RMS.saveRMSInt("isKiemduyet", 1);
					GameCanvas.isKiemduyet = false;
					break;
				}
				break;
			case 124:
				khamngoc(msg);
				break;
			case 125:
				switch (msg.reader().readByte())
				{
				case 0:
					addEffect(msg);
					break;
				case 1:
					getImgEffect(msg);
					break;
				case 2:
					getDataEffect(msg);
					break;
				}
				break;
			case 126:
			{
				bool num35 = msg.reader().readByte() != 0;
				GameCanvas.endDlg();
				if (!num35)
				{
					GameScr.instance.resetButton();
				}
				break;
			}
			case -4:
				mob = null;
				try
				{
					mob = Mob.get_Mob(msg.reader().readUnsignedByte());
				}
				catch (Exception)
				{
				}
				if (mob == null || mob.status == 0 || mob.status == 0)
				{
					break;
				}
				mob.startDie();
				try
				{
					int num33 = msg.reader().readInt();
					if (num33 < 0)
					{
						num33 = Res.abs(num33) + 32767;
					}
					if (msg.reader().readBoolean())
					{
						GameScr.startFlyText("-" + num33, mob.x, mob.y - mob.h, 0, -2, mFont.FATAL);
					}
					else
					{
						GameScr.startFlyText("-" + num33, mob.x, mob.y - mob.h, 0, -2, mFont.ORANGE);
					}
					ItemMap itemMap = new ItemMap(msg.reader().readShort(), msg.reader().readShort(), mob.x, mob.y, msg.reader().readShort(), msg.reader().readShort());
					GameScr.vItemMap.addElement(itemMap);
					if (Res.abs(itemMap.y - Char.getMyChar().cy) < 24 && Res.abs(itemMap.x - Char.getMyChar().cx) < 24)
					{
						Char.getMyChar().charFocus = null;
					}
				}
				catch (Exception)
				{
				}
				break;
			case -2:
			{
				GameCanvas.debug("SA87", 2);
				mob = null;
				try
				{
					mob = Mob.get_Mob(msg.reader().readUnsignedByte());
				}
				catch (Exception)
				{
				}
				int charId = msg.reader().readInt();
				int num8 = msg.reader().readInt();
				GameCanvas.debug("SA87x1", 2);
				if (mob != null)
				{
					GameCanvas.debug("SA87x2", 2);
					Char char10 = GameScr.findCharInMap(charId);
					if (char10 == null)
					{
						return;
					}
					GameCanvas.debug("SA87x3", 2);
					mob.dame = char10.cHP - num8;
					char10.cHpNew = num8;
					GameCanvas.debug("SA87x4", 2);
					try
					{
						char10.cMP = msg.reader().readInt();
					}
					catch (Exception)
					{
					}
					GameCanvas.debug("SA87x5", 2);
					if (mob.isBusyAttackSomeOne)
					{
						char10.doInjure(mob.dame, 0, isBoss: false, -1);
						mob.attackOtherInRange();
					}
					else
					{
						mob.setAttack(char10);
					}
					short idSkill_atk = msg.reader().readShort();
					sbyte typeAtk = msg.reader().readByte();
					sbyte typeTool = msg.reader().readByte();
					mob.setTypeAtk(idSkill_atk, typeAtk, typeTool);
					GameCanvas.debug("SA87x6", 2);
				}
				break;
			}
			case 65:
			{
				GameCanvas.debug("SA48", 2);
				Char char8 = GameScr.findCharInMap(msg.reader().readInt());
				if (char8 != null)
				{
					GameCanvas.startYesNoDlg(char8.cName + " " + mResources.INVITETEST, 88812, char8, 8882, null);
				}
				break;
			}
			case 94:
				GameCanvas.debug("SA577", 2);
				requestItemPlayer(msg);
				break;
			case 102:
			{
				GameCanvas.debug("SA74565", 2);
				Item item = Char.getMyChar().arrItemBag[msg.reader().readByte()];
				if (item != null)
				{
					GameScr.itemSell = item;
				}
				Char.getMyChar().xu = msg.reader().readInt();
				if (GameScr.itemSell != null)
				{
					if (GameScr.itemSell.template.type == 16)
					{
						GameScr.hpPotion -= GameScr.itemSell.quantity;
					}
					if (GameScr.itemSell.template.type == 17)
					{
						GameScr.mpPotion -= GameScr.itemSell.quantity;
					}
					Char.getMyChar().arrItemBag[GameScr.itemSell.indexUI] = null;
					GameScr.itemSell = null;
					GameScr.gI().resetButton();
					InfoMe.addInfo(mResources.SALE_INFO);
				}
				GameCanvas.endDlg();
				break;
			}
			case 118:
			{
				string text = msg.reader().readUTF();
				RMS.saveRMSString("acc", text);
				string text2 = msg.reader().readUTF();
				RMS.saveRMSString("pass", text2);
				SelectServerScr.uname = text;
				SelectServerScr.pass = text2;
				SelectServerScr.unameChange = string.Empty;
				SelectServerScr.passChange = string.Empty;
				if (!text.StartsWith("tmpusr"))
				{
					GameScr.gI().switchToMe();
				}
				break;
			}
			}
			GameCanvas.debug("SA92", 2);
		}
		catch (Exception ex40)
		{
			Out.println("loi tai cmd " + msg.command + " ly do >> " + ex40.ToString());
		}
		finally
		{
			msg?.cleanup();
		}
	}

	private void createItem(myReader d)
	{
		GameScr.vcItem = d.readByte();
		GameScr.iOptionTemplates = new ItemOptionTemplate[d.readUnsignedByte()];
		for (int i = 0; i < GameScr.iOptionTemplates.Length; i++)
		{
			GameScr.iOptionTemplates[i] = new ItemOptionTemplate();
			GameScr.iOptionTemplates[i].id = i;
			GameScr.iOptionTemplates[i].name = d.readUTF();
			GameScr.iOptionTemplates[i].type = d.readByte();
		}
		int num = d.readShort();
		for (int j = 0; j < num; j++)
		{
			ItemTemplate it = new ItemTemplate((short)j, d.readByte(), d.readByte(), d.readUTF(), d.readUTF(), d.readByte(), d.readShort(), d.readShort(), d.readBoolean());
			ItemTemplates.add(it);
		}
	}

	private void createSkill(myReader d)
	{
		GameScr.vcSkill = d.readByte();
		GameScr.sOptionTemplates = new SkillOptionTemplate[d.readByte()];
		for (int i = 0; i < GameScr.sOptionTemplates.Length; i++)
		{
			GameScr.sOptionTemplates[i] = new SkillOptionTemplate();
			GameScr.sOptionTemplates[i].id = i;
			GameScr.sOptionTemplates[i].name = d.readUTF();
		}
		GameScr.nClasss = new NClass[d.readUnsignedByte()];
		for (int j = 0; j < GameScr.nClasss.Length; j++)
		{
			GameScr.nClasss[j] = new NClass();
			GameScr.nClasss[j].classId = j;
			GameScr.nClasss[j].name = d.readUTF();
			GameScr.nClasss[j].skillTemplates = new SkillTemplate[d.readByte()];
			for (int k = 0; k < GameScr.nClasss[j].skillTemplates.Length; k++)
			{
				GameScr.nClasss[j].skillTemplates[k] = new SkillTemplate();
				GameScr.nClasss[j].skillTemplates[k].id = d.readByte();
				GameScr.nClasss[j].skillTemplates[k].name = d.readUTF();
				GameScr.nClasss[j].skillTemplates[k].maxPoint = d.readByte();
				GameScr.nClasss[j].skillTemplates[k].type = d.readByte();
				GameScr.nClasss[j].skillTemplates[k].iconId = d.readShort();
				int lineWidth = 150;
				if (GameCanvas.w == 128 || GameCanvas.h <= 208)
				{
					lineWidth = 100;
				}
				GameScr.nClasss[j].skillTemplates[k].description = mFont.tahoma_7_white.splitFontArray(d.readUTF(), lineWidth);
				GameScr.nClasss[j].skillTemplates[k].skills = new Skill[d.readByte()];
				for (int l = 0; l < GameScr.nClasss[j].skillTemplates[k].skills.Length; l++)
				{
					GameScr.nClasss[j].skillTemplates[k].skills[l] = new Skill();
					GameScr.nClasss[j].skillTemplates[k].skills[l].skillId = d.readShort();
					GameScr.nClasss[j].skillTemplates[k].skills[l].template = GameScr.nClasss[j].skillTemplates[k];
					GameScr.nClasss[j].skillTemplates[k].skills[l].point = d.readByte();
					GameScr.nClasss[j].skillTemplates[k].skills[l].level = d.readByte();
					GameScr.nClasss[j].skillTemplates[k].skills[l].manaUse = d.readShort();
					GameScr.nClasss[j].skillTemplates[k].skills[l].coolDown = d.readInt();
					GameScr.nClasss[j].skillTemplates[k].skills[l].dx = d.readShort();
					GameScr.nClasss[j].skillTemplates[k].skills[l].dy = d.readShort();
					GameScr.nClasss[j].skillTemplates[k].skills[l].maxFight = d.readByte();
					GameScr.nClasss[j].skillTemplates[k].skills[l].options = new SkillOption[d.readByte()];
					for (int m = 0; m < GameScr.nClasss[j].skillTemplates[k].skills[l].options.Length; m++)
					{
						GameScr.nClasss[j].skillTemplates[k].skills[l].options[m] = new SkillOption();
						GameScr.nClasss[j].skillTemplates[k].skills[l].options[m].param = d.readShort();
						GameScr.nClasss[j].skillTemplates[k].skills[l].options[m].optionTemplate = GameScr.sOptionTemplates[d.readByte()];
					}
					Skills.add(GameScr.nClasss[j].skillTemplates[k].skills[l]);
				}
			}
		}
	}

	private void createMap(myReader d)
	{
		GameScr.vcMap = d.readByte();
		TileMap.mapNames = new string[d.readUnsignedByte()];
		for (int i = 0; i < TileMap.mapNames.Length; i++)
		{
			TileMap.mapNames[i] = d.readUTF();
		}
		Npc.arrNpcTemplate = new NpcTemplate[d.readByte()];
		for (sbyte b = 0; b < Npc.arrNpcTemplate.Length; b++)
		{
			Npc.arrNpcTemplate[b] = new NpcTemplate();
			Npc.arrNpcTemplate[b].npcTemplateId = b;
			Npc.arrNpcTemplate[b].name = d.readUTF();
			Npc.arrNpcTemplate[b].headId = d.readShort();
			Npc.arrNpcTemplate[b].bodyId = d.readShort();
			Npc.arrNpcTemplate[b].legId = d.readShort();
			Npc.arrNpcTemplate[b].menu = new string[d.readByte()][];
			for (int j = 0; j < Npc.arrNpcTemplate[b].menu.Length; j++)
			{
				Npc.arrNpcTemplate[b].menu[j] = new string[d.readByte()];
				for (int k = 0; k < Npc.arrNpcTemplate[b].menu[j].Length; k++)
				{
					Npc.arrNpcTemplate[b].menu[j][k] = d.readUTF();
				}
			}
		}
		int num = d.readUnsignedByte();
		Mob.arrMobTemplate = new MobTemplate[num];
		for (int l = 0; l < num; l++)
		{
			Mob.arrMobTemplate[l] = new MobTemplate();
			Mob.arrMobTemplate[l].mobTemplateId = (short)l;
			Mob.arrMobTemplate[l].type = d.readByte();
			Mob.arrMobTemplate[l].name = d.readUTF();
			Mob.arrMobTemplate[l].hp = d.readInt();
			Mob.arrMobTemplate[l].rangeMove = d.readByte();
			Mob.arrMobTemplate[l].speed = d.readByte();
		}
	}

	private void createData(myReader d)
	{
		GameScr.vcData = d.readByte();
		RMS.saveRMS("nj_arrow", NinjaUtil.readByteArray(d));
		RMS.saveRMS("nj_effect", NinjaUtil.readByteArray(d));
		RMS.saveRMS("nj_image", NinjaUtil.readByteArray(d));
		RMS.saveRMS("nj_part", NinjaUtil.readByteArray(d));
		RMS.saveRMS("nj_skill", NinjaUtil.readByteArray(d));
		GameScr.tasks = new sbyte[d.readByte()][];
		GameScr.mapTasks = new sbyte[GameScr.tasks.Length][];
		for (int i = 0; i < GameScr.tasks.Length; i++)
		{
			GameScr.tasks[i] = new sbyte[d.readByte()];
			GameScr.mapTasks[i] = new sbyte[GameScr.tasks[i].Length];
			for (int j = 0; j < GameScr.tasks[i].Length; j++)
			{
				GameScr.tasks[i][j] = d.readByte();
				GameScr.mapTasks[i][j] = d.readByte();
			}
		}
		GameScr.exps = new long[d.readUnsignedByte()];
		for (int k = 0; k < GameScr.exps.Length; k++)
		{
			GameScr.exps[k] = d.readLong();
		}
		GameScr.crystals = new int[d.readByte()];
		for (int l = 0; l < GameScr.crystals.Length; l++)
		{
			GameScr.crystals[l] = d.readInt();
		}
		GameScr.upClothe = new int[d.readByte()];
		for (int m = 0; m < GameScr.upClothe.Length; m++)
		{
			GameScr.upClothe[m] = d.readInt();
		}
		GameScr.upAdorn = new int[d.readByte()];
		for (int n = 0; n < GameScr.upAdorn.Length; n++)
		{
			GameScr.upAdorn[n] = d.readInt();
		}
		GameScr.upWeapon = new int[d.readByte()];
		for (int num = 0; num < GameScr.upWeapon.Length; num++)
		{
			GameScr.upWeapon[num] = d.readInt();
		}
		GameScr.coinUpCrystals = new int[d.readByte()];
		for (int num2 = 0; num2 < GameScr.coinUpCrystals.Length; num2++)
		{
			GameScr.coinUpCrystals[num2] = d.readInt();
		}
		GameScr.coinUpClothes = new int[d.readByte()];
		for (int num3 = 0; num3 < GameScr.coinUpClothes.Length; num3++)
		{
			GameScr.coinUpClothes[num3] = d.readInt();
		}
		GameScr.coinUpAdorns = new int[d.readByte()];
		for (int num4 = 0; num4 < GameScr.coinUpAdorns.Length; num4++)
		{
			GameScr.coinUpAdorns[num4] = d.readInt();
		}
		GameScr.coinUpWeapons = new int[d.readByte()];
		for (int num5 = 0; num5 < GameScr.coinUpWeapons.Length; num5++)
		{
			GameScr.coinUpWeapons[num5] = d.readInt();
		}
		GameScr.goldUps = new int[d.readByte()];
		for (int num6 = 0; num6 < GameScr.goldUps.Length; num6++)
		{
			GameScr.goldUps[num6] = d.readInt();
		}
		GameScr.maxPercents = new int[d.readByte()];
		for (int num7 = 0; num7 < GameScr.maxPercents.Length; num7++)
		{
			GameScr.maxPercents[num7] = d.readInt();
		}
		Effect.effTemplates = new EffectTemplate[d.readByte()];
		for (int num8 = 0; num8 < Effect.effTemplates.Length; num8++)
		{
			Effect.effTemplates[num8] = new EffectTemplate();
			Effect.effTemplates[num8].id = d.readByte();
			Effect.effTemplates[num8].type = d.readByte();
			Effect.effTemplates[num8].name = d.readUTF();
			Effect.effTemplates[num8].iconId = d.readShort();
		}
	}

	public static Image createImage(sbyte[] arr)
	{
		try
		{
			return Image.createImage(arr, 0, arr.Length);
		}
		catch (Exception)
		{
			Out.println("loi tao hinh tai createImage cua controler");
		}
		return null;
	}

	public int[] arraysbyte2Int(sbyte[] b)
	{
		int[] array = new int[b.Length];
		for (int i = 0; i < b.Length; i++)
		{
			int num = b[i];
			if (num < 0)
			{
				num += 256;
			}
			array[i] = num;
		}
		return array;
	}

	public void loadInfoMap(Message msg)
	{
		try
		{
			Char.getMyChar().cx = (Char.getMyChar().cxSend = (Char.getMyChar().cxFocus = msg.reader().readShort()));
			Char.getMyChar().cy = (Char.getMyChar().cySend = (Char.getMyChar().cyFocus = msg.reader().readShort()));
			int num = msg.reader().readByte();
			for (int i = 0; i < num; i++)
			{
				TileMap.vGo.addElement(new Waypoint(msg.reader().readShort(), msg.reader().readShort(), msg.reader().readShort(), msg.reader().readShort()));
			}
			num = msg.reader().readByte();
			for (sbyte b = 0; b < num; b++)
			{
				Mob mob = new Mob(b, msg.reader().readBoolean(), msg.reader().readBoolean(), msg.reader().readBoolean(), msg.reader().readBoolean(), msg.reader().readBoolean(), msg.reader().readUnsignedByte(), msg.reader().readByte(), msg.reader().readInt(), msg.reader().readUnsignedByte(), msg.reader().readInt(), msg.reader().readShort(), msg.reader().readShort(), msg.reader().readByte(), msg.reader().readByte(), msg.reader().readBoolean(), removeWhenDie: false);
				if (Mob.arrMobTemplate[mob.templateId].type != 0)
				{
					if (b % 3 == 0)
					{
						mob.dir = -1;
					}
					else
					{
						mob.dir = 1;
					}
					mob.x += 10 - b % 20;
				}
				GameScr.vMob.addElement(mob);
			}
			num = msg.reader().readByte();
			for (byte b2 = 0; b2 < num; b2++)
			{
				GameScr.vBuNhin.addElement(new BuNhin(msg.reader().readUTF(), msg.reader().readShort(), msg.reader().readShort()));
			}
			num = msg.reader().readByte();
			for (int j = 0; j < num; j++)
			{
				GameScr.vNpc.addElement(new Npc(j, msg.reader().readByte(), msg.reader().readShort(), msg.reader().readShort(), msg.reader().readByte()));
			}
			num = msg.reader().readByte();
			for (int k = 0; k < num; k++)
			{
				ItemMap itemMap = new ItemMap(msg.reader().readShort(), msg.reader().readShort(), msg.reader().readShort(), msg.reader().readShort());
				bool flag = false;
				for (int l = 0; l < GameScr.vItemMap.size(); l++)
				{
					if (((ItemMap)GameScr.vItemMap.elementAt(l)).itemMapID == itemMap.itemMapID)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					GameScr.vItemMap.addElement(itemMap);
				}
			}
			GameScr.loadCamera(fullScreen: false);
			try
			{
				TileMap.mapName1 = null;
				TileMap.mapName1 = msg.reader().readUTF();
				TileMap.mapName = TileMap.mapName1;
			}
			catch (Exception)
			{
			}
			try
			{
				TileMap.locationStand.clear();
				int num2 = msg.reader().readUnsignedByte();
				for (int m = 0; m < num2; m++)
				{
					int num3 = msg.reader().readUnsignedByte();
					string k2 = (short)(msg.reader().readUnsignedByte() * TileMap.tmw + num3) + string.Empty;
					TileMap.locationStand.put(k2, "location");
				}
			}
			catch (Exception ex2)
			{
				ex2.ToString();
			}
			TileMap.loadMap(TileMap.tileID);
			Char.getMyChar().cvx = 0;
			Char.getMyChar().statusMe = 4;
			GameCanvas.loadBG(TileMap.bgID);
			Char.isLockKey = false;
			Char.ischangingMap = false;
			GameCanvas.clearKeyHold();
			GameCanvas.clearKeyPressed();
			if (TileMap.MapMod == TileMap.mapID || !TileMap.IsDangGoMap)
			{
				GameScr.gI().switchToMe();
				InfoDlg.hide();
				InfoDlg.show(TileMap.mapName, mResources.ZONE + " " + TileMap.zoneID, 30);
				Party.refreshAll();
				GameCanvas.endDlg();
				GameScr.gI().loadGameScr();
			}
			GameCanvas.isLoading = false;
		}
		catch (Exception ex3)
		{
			Debug.Log("EROR: " + ex3.Message);
		}
		TileMap.HuyLockMap();
		if (!Code.FirstZone)
		{
			if (map == TileMap.mapID)
			{
				Code.timeChangeZone = mSystem.currentTimeMillis();
			}
			else
			{
				Code.timeChangeZone = mSystem.currentTimeMillis() - 5000;
			}
		}
		Code.FirstZone = false;
		map = TileMap.mapID;
	}

	public void messageNotMap(Message msg)
	{
		GameCanvas.debug("SA6", 2);
		try
		{
			switch (msg.reader().readByte())
			{
			case -59:
				GameMidlet.instance.CheckPerGPS();
				break;
			case -98:
				Char.getMyChar().clearTask();
				break;
			case -99:
			{
				GameCanvas.input2Dlg.setTitle(mResources.SERI_NUM, mResources.CARD_CODE);
				string info = msg.reader().readUTF();
				GameCanvas.input2Dlg.show(info, new Command(mResources.CLOSE, GameCanvas.instance, 8882, null), new Command(mResources.CHARGE, GameCanvas.instance, 88816, null), TField.INPUT_TYPE_ANY, TField.INPUT_TYPE_NUMERIC);
				break;
			}
			case -97:
			{
				GameCanvas.isLoading = false;
				GameCanvas.endDlg();
				int num6 = msg.reader().readInt();
				GameCanvas.inputDlg.show(mResources.NAME_CHANGE, new Command("OK", GameCanvas.instance, 88829, num6), TField.INPUT_TYPE_ANY);
				break;
			}
			case -115:
			{
				int id = msg.reader().readInt();
				sbyte[] data = NinjaUtil.readByteArray(msg);
				SmallImage.reciveImage(id, data);
				break;
			}
			case -117:
				Char.getMyChar().cPk = msg.reader().readByte();
				Info.addInfo(mResources.PK_NOW + " " + Char.getMyChar().cPk, 15, mFont.tahoma_7_yellow);
				Char.getMyChar().callEffTask(21);
				break;
			case -96:
				Char.getMyChar().cClanName = msg.reader().readUTF();
				Char.getMyChar().ctypeClan = 4;
				Char.getMyChar().luong = msg.reader().readInt();
				Char.getMyChar().callEffTask(21);
				break;
			case -113:
				Out.println("vao REQUEST_CLAN_INFO roi ");
				if (Char.clan == null)
				{
					Char.clan = new Clan();
				}
				Char.clan.name = msg.reader().readUTF();
				Char.clan.main_name = msg.reader().readUTF();
				Char.clan.assist_name = msg.reader().readUTF();
				Char.clan.total = msg.reader().readShort();
				Char.clan.openDun = msg.reader().readByte();
				Char.clan.level = msg.reader().readByte();
				Char.clan.exp = msg.reader().readInt();
				Char.clan.expNext = msg.reader().readInt();
				Char.clan.coin = msg.reader().readInt();
				Char.clan.freeCoin = msg.reader().readInt();
				Char.clan.coinUp = msg.reader().readInt();
				Char.clan.reg_date = msg.reader().readUTF();
				Char.clan.alert = msg.reader().readUTF();
				Char.clan.use_card = msg.reader().readInt();
				Char.clan.itemLevel = msg.reader().readByte();
				break;
			case -93:
			{
				int num17 = msg.reader().readInt();
				if (num17 == Char.getMyChar().charID)
				{
					GameScr.vClan.removeAllElements();
					Char.getMyChar().cClanName = string.Empty;
					Char.getMyChar().ctypeClan = -1;
					Char.clan = null;
				}
				else
				{
					GameScr.vClan.removeAllElements();
					Char char3 = GameScr.findCharInMap(num17);
					char3.cClanName = string.Empty;
					char3.ctypeClan = -1;
				}
				break;
			}
			case -114:
				if (Char.clan == null)
				{
					Char.clan = new Clan();
				}
				Char.clan.writeLog(msg.reader().readUTF());
				break;
			case -62:
				Char.clan.itemLevel = msg.reader().readByte();
				break;
			case -81:
				Char.pointChienTruong = msg.reader().readShort();
				break;
			case -77:
				TileMap.bgID = msg.reader().readByte();
				GameCanvas.loadBG(TileMap.bgID);
				break;
			case -70:
			{
				string replacement = msg.reader().readUTF();
				GameCanvas.startYesNoDlg(NinjaUtil.replace(mResources.INVITE_TO_CBT, "#", replacement), new Command(mResources.YES, GameCanvas.instance, 88842, null), new Command(mResources.NO, GameCanvas.instance, 8882, null));
				break;
			}
			case -72:
			{
				GameScr.gI().yenValue = new string[9];
				GameScr.arrItemSprin = new short[9];
				if (GameScr.indexSelect < 0 || GameScr.indexSelect > 8)
				{
					GameScr.indexSelect = (GameScr.indexCard = 0);
				}
				if (GameScr.indexSelect < 0 || GameScr.indexSelect > 8)
				{
					GameScr.indexSelect = (GameScr.indexCard = 0);
				}
				for (int k = 0; k < 9; k++)
				{
					GameScr.arrItemSprin[k] = msg.reader().readShort();
					GameScr.gI().yenValue[k] = GameScr.gI().YenCards[NinjaUtil.randomNumber(6)];
				}
				GameScr.gI().left = new Command(mResources.CONTINUE, null, 1506, null);
				GameScr.gI().timePoint = mSystem.getCurrentTimeMillis();
				GameScr.gI().numSprinLeft--;
				GameCanvas.endDlg();
				break;
			}
			case -88:
			{
				GameScr.gI().resetButton();
				Item item = Char.getMyChar().arrItemBag[msg.reader().readByte()];
				item.clearExpire();
				item.isLock = true;
				item.upgrade = msg.reader().readByte();
				Item item2 = Char.getMyChar().arrItemBag[msg.reader().readByte()];
				item2.clearExpire();
				item2.isLock = true;
				item2.upgrade = msg.reader().readByte();
				Info.addInfo(mResources.CONVERT_OK, 20, mFont.tahoma_7b_yellow);
				break;
			}
			case -112:
			{
				GameScr.vClan.removeAllElements();
				int num2 = msg.reader().readShort();
				for (int i = 0; i < num2; i++)
				{
					GameScr.vClan.addElement(new Member(msg.reader().readByte(), msg.reader().readByte(), msg.reader().readByte(), msg.reader().readUTF(), msg.reader().readInt(), msg.reader().readBoolean()));
				}
				try
				{
					for (int j = 0; j < num2; j++)
					{
						((Member)GameScr.vClan.elementAt(j)).pointClanWeek = msg.reader().readInt();
					}
				}
				catch (Exception)
				{
				}
				GameScr.gI().sortClan();
				break;
			}
			case -111:
			{
				Char.clan.items = new Item[30];
				int num18 = msg.reader().readByte();
				for (int num19 = 0; num19 < num18; num19++)
				{
					Char.clan.items[num19] = new Item();
					Char.clan.items[num19].typeUI = 39;
					Char.clan.items[num19].indexUI = num19;
					Char.clan.items[num19].quantity = msg.reader().readShort();
					Char.clan.items[num19].template = ItemTemplates.get(msg.reader().readShort());
				}
				GameScr.gI().clearVecThanThu();
				sbyte b3 = msg.reader().readByte();
				for (int num20 = 0; num20 < b3; num20++)
				{
					string name = msg.reader().readUTF();
					short idIconItem = msg.reader().readShort();
					short idThanThu = msg.reader().readShort();
					int num21 = msg.reader().readInt();
					string str_trungno = string.Empty;
					MyVector myVector = new MyVector();
					int curExp = -1;
					int maxExp = -1;
					sbyte b4 = msg.reader().readByte();
					if (num21 >= 0)
					{
						str_trungno = msg.reader().readUTF();
					}
					else
					{
						for (int num22 = 0; num22 < b4; num22++)
						{
							string o = msg.reader().readUTF();
							myVector.addElement(o);
						}
						curExp = msg.reader().readInt();
						maxExp = msg.reader().readInt();
					}
					sbyte stars = msg.reader().readByte();
					GameScr.gI().addInfo_ThanThu(new Clan_ThanThu(name, stars, idIconItem, idThanThu, num21, str_trungno, myVector, curExp, maxExp));
				}
				break;
			}
			case -116:
				Char.getMyChar().xu = msg.reader().readInt();
				Char.clan.coin = msg.reader().readInt();
				break;
			case -90:
				Char.getMyChar().xu = msg.reader().readInt();
				GameScr.gI().resetButton();
				break;
			case -86:
				GameCanvas.endDlg();
				GameScr.gI().resetButton();
				InfoMe.addInfo(msg.reader().readUTF(), 20, mFont.tahoma_7_yellow);
				break;
			case -106:
				GameScr.typeActive = msg.reader().readByte();
				Out.println("load Me Active: " + GameScr.typeActive);
				break;
			case -84:
				Char.pointPB = msg.reader().readShort();
				break;
			case -80:
				GameScr.gI().showAlert(mResources.RESULT, msg.reader().readUTF(), withMenuShow: false);
				if (msg.reader().readBoolean())
				{
					GameScr.gI().left = new Command(mResources.REWARD, 2000);
				}
				break;
			case -83:
			{
				int num13 = msg.reader().readShort();
				int num14 = msg.reader().readShort();
				int num15 = msg.reader().readByte();
				int num16 = msg.reader().readShort();
				if (num13 == 0)
				{
					GameScr.gI().showAlert(mResources.REVIEW, "          " + mResources.EMPTY_INFO, withMenuShow: false);
					break;
				}
				string text = mResources.PROPERTY + ": " + num13 + "\n\n";
				string text2;
				if (num14 == 0)
				{
					text = text + mResources.NOT_FINISH + "\n\n";
				}
				else
				{
					text2 = text;
					text = text2 + mResources.TIME_FINISH + ": " + NinjaUtil.getTime(num14) + "\n\n";
				}
				text2 = text;
				text = text2 + mResources.TEAMWORK + ": " + num15 + "\n\n";
				text2 = text;
				text = text2 + mResources.REWARD + ": " + num16 + " " + mResources.LUCKY_GIFT + "\n\n";
				GameScr.gI().showAlert(mResources.REVIEW, text, withMenuShow: false);
				if (num16 > 0)
				{
					GameScr.gI().left = new Command(mResources.REWARD, 1000);
				}
				break;
			}
			case -95:
				if (Char.clan != null)
				{
					Char.clan.alert = msg.reader().readUTF();
				}
				break;
			case -126:
			{
				GameCanvas.debug("SA7", 2);
				int num4 = msg.reader().readByte();
				LoginScr.isLoggingIn = false;
				SelectCharScr.gI().initSelectChar();
				for (sbyte b = 0; b < num4; b++)
				{
					SelectCharScr.gI().gender[b] = msg.reader().readByte();
					SelectCharScr.gI().name[b] = msg.reader().readUTF();
					SelectCharScr.gI().phai[b] = msg.reader().readUTF();
					SelectCharScr.gI().level[b] = msg.reader().readUnsignedByte();
					SelectCharScr.gI().parthead[b] = msg.reader().readShort();
					SelectCharScr.gI().partWp[b] = msg.reader().readShort();
					SelectCharScr.gI().partbody[b] = msg.reader().readShort();
					SelectCharScr.gI().partleg[b] = msg.reader().readShort();
					if (SelectCharScr.gI().partWp[b] == -1)
					{
						SelectCharScr.gI().partWp[b] = 15;
					}
					if (SelectCharScr.gI().partbody[b] == -1)
					{
						if (SelectCharScr.gI().gender[b] == 0)
						{
							SelectCharScr.gI().partbody[b] = 10;
						}
						else
						{
							SelectCharScr.gI().partbody[b] = 1;
						}
					}
					if (SelectCharScr.gI().partleg[b] == -1)
					{
						if (SelectCharScr.gI().gender[b] == 0)
						{
							SelectCharScr.gI().partleg[b] = 9;
						}
						else
						{
							SelectCharScr.gI().partleg[b] = 0;
						}
					}
				}
				SelectCharScr.gI().switchToMe();
				GameCanvas.endDlg();
				break;
			}
			case -123:
				GameCanvas.debug("SA8", 2);
				GameScr.vsData = msg.reader().readByte();
				GameScr.vsMap = msg.reader().readByte();
				GameScr.vsSkill = msg.reader().readByte();
				GameScr.vsItem = msg.reader().readByte();
				if (GameScr.vsData != GameScr.vcData)
				{
					Service.gI().updateData();
				}
				else
				{
					try
					{
						DataInputStream dataInputStream = new DataInputStream(RMS.loadRMS("data"));
						createData(dataInputStream.r);
					}
					catch (Exception)
					{
						GameScr.vcData = -1;
						Service.gI().updateData();
					}
				}
				if (GameScr.vsMap != GameScr.vcMap)
				{
					Service.gI().updateMap();
				}
				else
				{
					try
					{
						DataInputStream dataInputStream2 = new DataInputStream(RMS.loadRMS("map"));
						createMap(dataInputStream2.r);
					}
					catch (Exception)
					{
						GameScr.vcMap = -1;
						Service.gI().updateMap();
					}
				}
				if (GameScr.vsSkill != GameScr.vcSkill)
				{
					Service.gI().updateSkill();
				}
				else
				{
					try
					{
						DataInputStream dataInputStream3 = new DataInputStream(RMS.loadRMS("skill"));
						createSkill(dataInputStream3.r);
					}
					catch (Exception)
					{
						GameScr.vcSkill = -1;
						Service.gI().updateSkill();
					}
				}
				if (GameScr.vsItem != GameScr.vcItem)
				{
					Service.gI().updateItem();
				}
				else
				{
					try
					{
						DataInputStream dataInputStream4 = new DataInputStream(RMS.loadRMS("item"));
						createItem(dataInputStream4.r);
					}
					catch (Exception)
					{
						GameScr.vcItem = -1;
						Service.gI().updateItem();
					}
				}
				if (GameScr.vsData == GameScr.vcData && GameScr.vsMap == GameScr.vcMap && GameScr.vsSkill == GameScr.vcSkill && GameScr.vsItem == GameScr.vcItem)
				{
					GameScr.gI().readEfect();
					GameScr.gI().readArrow();
					GameScr.gI().readSkill();
					Service.gI().clientOk();
				}
				CharPartInfo.doSetInfo(msg);
				break;
			case -122:
			{
				Out.println("GET UPDATE_DATA " + msg.reader().available() + " sbytes");
				msg.reader().mark(100000);
				createData(msg.reader());
				msg.reader().reset();
				sbyte[] data4 = new sbyte[msg.reader().available()];
				msg.reader().readFully(ref data4);
				RMS.saveRMS("data", data4);
				sbyte[] data5 = new sbyte[1] { GameScr.vcData };
				RMS.saveRMS("dataVersion", data5);
				if (GameScr.vsData == GameScr.vcData && GameScr.vsMap == GameScr.vcMap && GameScr.vsSkill == GameScr.vcSkill && GameScr.vsItem == GameScr.vcItem)
				{
					GameScr.gI().readEfect();
					GameScr.gI().readArrow();
					GameScr.gI().readSkill();
					Service.gI().clientOk();
				}
				break;
			}
			case -121:
			{
				Out.println("GET UPDATE_MAP " + msg.reader().available() + " sbytes");
				msg.reader().mark(100000);
				createMap(msg.reader());
				msg.reader().reset();
				sbyte[] data2 = new sbyte[msg.reader().available()];
				msg.reader().readFully(ref data2);
				RMS.saveRMS("map", data2);
				sbyte[] data3 = new sbyte[1] { GameScr.vcMap };
				RMS.saveRMS("mapVersion", data3);
				if (GameScr.vsData == GameScr.vcData && GameScr.vsMap == GameScr.vcMap && GameScr.vsSkill == GameScr.vcSkill && GameScr.vsItem == GameScr.vcItem)
				{
					GameScr.gI().readEfect();
					GameScr.gI().readArrow();
					GameScr.gI().readSkill();
					Service.gI().clientOk();
				}
				break;
			}
			case -120:
			{
				Out.println("GET UPDATE_SKILL " + msg.reader().available() + " sbytes");
				msg.reader().mark(100000);
				createSkill(msg.reader());
				msg.reader().reset();
				sbyte[] data8 = new sbyte[msg.reader().available()];
				msg.reader().readFully(ref data8);
				if (Char.getMyChar().isHumanz())
				{
					RMS.saveRMS("skill", data8);
				}
				else
				{
					RMS.saveRMS("skillnhanban", data8);
				}
				sbyte[] data9 = new sbyte[1] { GameScr.vcSkill };
				RMS.saveRMS("skillVersion", data9);
				if (GameScr.vsData == GameScr.vcData && GameScr.vsMap == GameScr.vcMap && GameScr.vsSkill == GameScr.vcSkill && GameScr.vsItem == GameScr.vcItem)
				{
					GameScr.gI().readEfect();
					GameScr.gI().readArrow();
					GameScr.gI().readSkill();
					Service.gI().clientOk();
				}
				break;
			}
			case -119:
			{
				Out.println("GET UPDATE_ITEM " + msg.reader().available() + " sbytes");
				msg.reader().mark(100000);
				createItem(msg.reader());
				msg.reader().reset();
				sbyte[] data6 = new sbyte[msg.reader().available()];
				msg.reader().readFully(ref data6);
				RMS.saveRMS("item", data6);
				sbyte[] data7 = new sbyte[1] { GameScr.vcItem };
				RMS.saveRMS("itemVersion", data7);
				if (GameScr.vsData == GameScr.vcData && GameScr.vsMap == GameScr.vcMap && GameScr.vsSkill == GameScr.vcSkill && GameScr.vsItem == GameScr.vcItem)
				{
					GameScr.gI().readEfect();
					GameScr.gI().readArrow();
					GameScr.gI().readSkill();
					Service.gI().clientOk();
				}
				break;
			}
			case -108:
			{
				int num7 = msg.reader().readShort();
				try
				{
					sbyte typeFly = msg.reader().readByte();
					Mob.arrMobTemplate[num7].typeFly = typeFly;
				}
				catch (Exception)
				{
				}
				sbyte b2 = msg.reader().readByte();
				Mob.arrMobTemplate[num7].imgs = new Image[b2];
				if (num7 == 98 || num7 == 99)
				{
					Mob.arrMobTemplate[num7].imgs = new Image[3];
					Image image = createImage(NinjaUtil.readByteArray(msg));
					for (int m = 0; m < Mob.arrMobTemplate[num7].imgs.Length; m++)
					{
						Mob.arrMobTemplate[num7].imgs[m] = image;
					}
				}
				else
				{
					for (int n = 0; n < Mob.arrMobTemplate[num7].imgs.Length; n++)
					{
						sbyte[] arr = NinjaUtil.readByteArray(msg);
						Mob.arrMobTemplate[num7].imgs[n] = createImage(arr);
					}
				}
				if (msg.reader().readBoolean())
				{
					int num8 = msg.reader().readByte();
					Mob.arrMobTemplate[num7].frameBossMove = new sbyte[num8];
					for (int num9 = 0; num9 < num8; num9++)
					{
						Mob.arrMobTemplate[num7].frameBossMove[num9] = msg.reader().readByte();
					}
					num8 = msg.reader().readByte();
					Mob.arrMobTemplate[num7].frameBossAttack = new sbyte[num8][];
					for (int num10 = 0; num10 < num8; num10++)
					{
						Mob.arrMobTemplate[num7].frameBossAttack[num10] = new sbyte[msg.reader().readByte()];
						for (int num11 = 0; num11 < Mob.arrMobTemplate[num7].frameBossAttack[num10].Length; num11++)
						{
							Mob.arrMobTemplate[num7].frameBossAttack[num10][num11] = msg.reader().readByte();
						}
					}
				}
				int num12 = msg.reader().readInt();
				if (num12 > 0)
				{
					if (num7 < 236)
					{
						readDataMobOld(msg, num7);
					}
					else
					{
						readDataMobNew(msg, num7);
					}
				}
				break;
			}
			case -109:
				try
				{
					GameCanvas.isLoading = true;
					TileMap.maps = null;
					TileMap.types = null;
					GameCanvas.debug("SA99", 2);
					TileMap.tmw = msg.reader().readByte();
					TileMap.tmh = msg.reader().readByte();
					TileMap.maps = new char[TileMap.tmw * TileMap.tmh];
					for (int l = 0; l < TileMap.maps.Length; l++)
					{
						int num5 = msg.reader().readByte();
						if (num5 < 0)
						{
							num5 += 256;
						}
						TileMap.maps[l] = (char)num5;
					}
					TileMap.types = new int[TileMap.maps.Length];
					msg = messWait;
					loadInfoMap(msg);
				}
				catch (Exception)
				{
					Out.println(" loi tai cmd  " + msg.command);
				}
				msg.cleanup();
				messWait.cleanup();
				msg = (messWait = null);
				break;
			case -107:
				GameCanvas.debug("SA10", 2);
				break;
			case -110:
				GameCanvas.debug("SA11", 2);
				break;
			case -67:
			{
				Mob mob = null;
				try
				{
					int iD = msg.reader().readUnsignedByte();
					mob = Mob.get_Mob(iD);
				}
				catch (Exception)
				{
				}
				if (mob == null)
				{
					break;
				}
				int num3 = msg.reader().readInt();
				if (num3 == Char.getMyChar().charID)
				{
					GameScr.vMobSoul.addElement(new MobSoul(mob.x, mob.y, Char.getMyChar()));
					break;
				}
				Char char2 = GameScr.findCharInMap(num3);
				if (char2 != null)
				{
					GameScr.vMobSoul.addElement(new MobSoul(mob.x, mob.y, char2));
				}
				break;
			}
			case -66:
			{
				int num = msg.reader().readInt();
				if (Char.getMyChar().charID == num)
				{
					GameScr.vMobSoul.addElement(new MobSoul(Char.getMyChar().cx, Char.getMyChar().cy));
					break;
				}
				Char @char = GameScr.findCharInMap(num);
				if (@char != null)
				{
					GameScr.vMobSoul.addElement(new MobSoul(@char.cx, @char.cy));
				}
				break;
			}
			case -125:
			case -124:
			case -118:
			case -105:
			case -104:
			case -103:
			case -102:
			case -101:
			case -100:
			case -94:
			case -92:
			case -91:
			case -89:
			case -87:
			case -85:
			case -82:
			case -79:
			case -78:
			case -76:
			case -75:
			case -74:
			case -73:
			case -71:
			case -69:
			case -68:
			case -65:
			case -64:
			case -63:
			case -61:
			case -60:
				break;
			}
		}
		catch (Exception)
		{
		}
		finally
		{
			msg?.cleanup();
		}
	}

	public void messageNotLogin(Message msg)
	{
		try
		{
			switch (msg.reader().readByte())
			{
			case -124:
			{
				string text = msg.reader().readUTF();
				string data = msg.reader().readUTF();
				GameMidlet.sendSMSRe(data, "sms://" + text, new Command(string.Empty, GameCanvas.gI(), 88825, null), new Command(string.Empty, GameCanvas.gI(), 88826, null));
				break;
			}
			case 2:
				RMS.clearRMS();
				break;
			}
		}
		catch (Exception)
		{
		}
		finally
		{
			msg?.cleanup();
		}
	}

	public void messageSubCommand(Message msg)
	{
		try
		{
			GameCanvas.debug("SA12", 2);
			sbyte b = msg.reader().readByte();
			Out.println("sub: " + b);
			switch (b)
			{
			case sbyte.MinValue:
			{
				GameCanvas.debug("SA27", 2);
				Char char14 = GameScr.findCharInMap(msg.reader().readInt());
				if (char14 != null)
				{
					char14.cHP = msg.reader().readInt();
					char14.cMaxHP = msg.reader().readInt();
					char14.clevel = msg.reader().readUnsignedByte();
				}
				break;
			}
			case -127:
			{
				if (DateTime.Now.Second == 61)
				{
					GameScr.vCharInMap.removeAllElements();
					GameScr.vItemMap.removeAllElements();
				}
				Code.bagshort = false;
				GameScr.loadImg();
				GameScr.currentCharViewInfo = Char.getMyChar();
				Char.getMyChar().charID = msg.reader().readInt();
				Char.getMyChar().cClanName = msg.reader().readUTF();
				if (!Char.getMyChar().cClanName.Equals(string.Empty))
				{
					Char.getMyChar().ctypeClan = msg.reader().readByte();
				}
				Char.getMyChar().ctaskId = msg.reader().readByte();
				Char.getMyChar().cgender = msg.reader().readByte();
				Char.getMyChar().head = msg.reader().readShort();
				Char.getMyChar().cspeed = msg.reader().readByte();
				Char.getMyChar().cName = msg.reader().readUTF();
				Char.getMyChar().cPk = msg.reader().readByte();
				Char.getMyChar().cTypePk = msg.reader().readByte();
				Char.getMyChar().cMaxHP = msg.reader().readInt();
				Char.getMyChar().cHP = msg.reader().readInt();
				Char.getMyChar().cMaxMP = msg.reader().readInt();
				Char.getMyChar().cMP = msg.reader().readInt();
				Char.getMyChar().cEXP = msg.reader().readLong();
				Char.getMyChar().cExpDown = msg.reader().readLong();
				GameScr.setLevel_Exp(Char.getMyChar().cEXP, value: true);
				Char.getMyChar().eff5BuffHp = msg.reader().readShort();
				Char.getMyChar().eff5BuffMp = msg.reader().readShort();
				Char.getMyChar().nClass = GameScr.nClasss[msg.reader().readByte()];
				Char.getMyChar().pPoint = msg.reader().readShort();
				Char.getMyChar().potential[0] = msg.reader().readShort();
				Char.getMyChar().potential[1] = msg.reader().readShort();
				Char.getMyChar().potential[2] = msg.reader().readInt();
				Char.getMyChar().potential[3] = msg.reader().readInt();
				Char.getMyChar().sPoint = msg.reader().readShort();
				Char.getMyChar().vSkill.removeAllElements();
				Char.getMyChar().vSkillFight.removeAllElements();
				sbyte b8 = msg.reader().readByte();
				for (byte b9 = 0; b9 < (byte)b8; b9++)
				{
					Skill skill3 = Skills.get(msg.reader().readShort());
					if (Char.getMyChar().myskill == null)
					{
						Char.getMyChar().myskill = skill3;
					}
					Char.getMyChar().vSkill.addElement(skill3);
					if ((skill3.template.type == 1 || skill3.template.type == 4 || skill3.template.type == 2 || skill3.template.type == 3) && (skill3.template.maxPoint == 0 || (skill3.template.maxPoint > 0 && skill3.point > 0)))
					{
						if (skill3.template.id == Char.getMyChar().skillTemplateId)
						{
							Service.gI().selectSkill(Char.getMyChar().skillTemplateId);
						}
						Char.getMyChar().vSkillFight.addElement(skill3);
					}
				}
				GameScr.gI().sortSkill();
				Char.getMyChar().xu = msg.reader().readInt();
				Char.getMyChar().yen = msg.reader().readInt();
				Char.getMyChar().luong = msg.reader().readInt();
				Char.getMyChar().arrItemBag = new Item[msg.reader().readUnsignedByte()];
				GameScr.hpPotion = (GameScr.mpPotion = 0);
				for (int num28 = 0; num28 < Char.getMyChar().arrItemBag.Length; num28++)
				{
					short num29 = msg.reader().readShort();
					if (num29 != -1)
					{
						Char.getMyChar().arrItemBag[num28] = new Item();
						Char.getMyChar().arrItemBag[num28].typeUI = 3;
						Char.getMyChar().arrItemBag[num28].indexUI = num28;
						Char.getMyChar().arrItemBag[num28].template = ItemTemplates.get(num29);
						Char.getMyChar().arrItemBag[num28].isLock = msg.reader().readBoolean();
						if (Char.getMyChar().arrItemBag[num28].isTypeBody() || Char.getMyChar().arrItemBag[num28].isTypeMounts() || Char.getMyChar().arrItemBag[num28].isTypeNgocKham())
						{
							Char.getMyChar().arrItemBag[num28].upgrade = msg.reader().readByte();
						}
						Char.getMyChar().arrItemBag[num28].isExpires = msg.reader().readBoolean();
						Char.getMyChar().arrItemBag[num28].quantity = msg.reader().readUnsignedShort();
						if (Char.getMyChar().arrItemBag[num28].template.type == 16)
						{
							GameScr.hpPotion += Char.getMyChar().arrItemBag[num28].quantity;
						}
						if (Char.getMyChar().arrItemBag[num28].template.type == 17)
						{
							GameScr.mpPotion += Char.getMyChar().arrItemBag[num28].quantity;
						}
						if (Char.getMyChar().arrItemBag[num28].template.id == 340)
						{
							GameScr.gI().numSprinLeft += Char.getMyChar().arrItemBag[num28].quantity;
						}
					}
				}
				Char.getMyChar().arrItemBody = new Item[32];
				try
				{
					Char.getMyChar().setDefaultPart();
					for (int num30 = 0; num30 < 16; num30++)
					{
						short num31 = msg.reader().readShort();
						if (num31 != -1)
						{
							ItemTemplate itemTemplate3 = ItemTemplates.get(num31);
							int type2 = itemTemplate3.type;
							Char.getMyChar().arrItemBody[type2] = new Item();
							Char.getMyChar().arrItemBody[type2].indexUI = type2;
							Char.getMyChar().arrItemBody[type2].typeUI = 5;
							Char.getMyChar().arrItemBody[type2].template = itemTemplate3;
							Char.getMyChar().arrItemBody[type2].isLock = true;
							Char.getMyChar().arrItemBody[type2].upgrade = msg.reader().readByte();
							Char.getMyChar().arrItemBody[type2].sys = msg.reader().readByte();
							switch (type2)
							{
							case 6:
								Char.getMyChar().leg = Char.getMyChar().arrItemBody[type2].template.part;
								break;
							case 2:
								Char.getMyChar().body = Char.getMyChar().arrItemBody[type2].template.part;
								break;
							case 1:
								Char.getMyChar().wp = Char.getMyChar().arrItemBody[type2].template.part;
								break;
							}
						}
					}
				}
				catch (Exception)
				{
				}
				Char.getMyChar().isHuman = msg.reader().readBoolean();
				Char.getMyChar().isNhanban = msg.reader().readBoolean();
				short[] array4 = new short[4]
				{
					msg.reader().readShort(),
					msg.reader().readShort(),
					msg.reader().readShort(),
					msg.reader().readShort()
				};
				if (array4[0] > -1)
				{
					Char.getMyChar().head = array4[0];
				}
				if (array4[1] > -1)
				{
					Char.getMyChar().wp = array4[1];
				}
				if (array4[2] > -1)
				{
					Char.getMyChar().body = array4[2];
				}
				if (array4[3] > -1)
				{
					Char.getMyChar().leg = array4[3];
				}
				if (Char.getMyChar().isHuman)
				{
					GameScr.gI().loadSkillShortcut();
				}
				else if (Char.getMyChar().isNhanban)
				{
					GameScr.gI().loadSkillShortcutNhanban();
				}
				Char.getMyChar().statusMe = 4;
				GameScr.isViewClanInvite = RMS.loadRMSInt(Char.getMyChar().cName + "vci") >= 1;
				short[] array5 = new short[10];
				try
				{
					for (int num32 = 0; num32 < 10; num32++)
					{
						array5[num32] = msg.reader().readShort();
					}
				}
				catch (Exception)
				{
					array5 = null;
				}
				if (array5 != null)
				{
					Char.getMyChar().setThoiTrang(array5);
				}
				try
				{
					for (int num33 = 0; num33 < 16; num33++)
					{
						short num34 = msg.reader().readShort();
						if (num34 != -1)
						{
							ItemTemplate itemTemplate4 = ItemTemplates.get(num34);
							int num35 = itemTemplate4.type + 16;
							Char.getMyChar().arrItemBody[num35] = new Item();
							Char.getMyChar().arrItemBody[num35].indexUI = num35;
							Char.getMyChar().arrItemBody[num35].typeUI = 5;
							Char.getMyChar().arrItemBody[num35].template = itemTemplate4;
							Char.getMyChar().arrItemBody[num35].isLock = true;
							Char.getMyChar().arrItemBody[num35].upgrade = msg.reader().readByte();
							Char.getMyChar().arrItemBody[num35].sys = msg.reader().readByte();
							switch (num35)
							{
							case 6:
								Char.getMyChar().leg = Char.getMyChar().arrItemBody[num35].template.part;
								break;
							case 2:
								Char.getMyChar().body = Char.getMyChar().arrItemBody[num35].template.part;
								break;
							case 1:
								Char.getMyChar().wp = Char.getMyChar().arrItemBody[num35].template.part;
								break;
							}
						}
					}
				}
				catch (Exception)
				{
				}
				if (Char.getMyChar().isHumanz())
				{
					DataInputStream dataInputStream = new DataInputStream(RMS.loadRMS("skill"));
					createSkill(dataInputStream.r);
				}
				else
				{
					DataInputStream dataInputStream2 = new DataInputStream(RMS.loadRMS("skill"));
					createSkill(dataInputStream2.r);
				}
				Service.gI().loadRMS("KSkill");
				Service.gI().loadRMS("OSkill");
				Service.gI().loadRMS("CSkill");
				break;
			}
			case -126:
				Char.getMyChar().readParam(msg, "Cmd.ME_LOAD_SKILL");
				Char.getMyChar().potential[0] = msg.reader().readShort();
				Char.getMyChar().potential[1] = msg.reader().readShort();
				Char.getMyChar().potential[2] = msg.reader().readInt();
				Char.getMyChar().potential[3] = msg.reader().readInt();
				Char.getMyChar().callEffTask(61);
				Char.getMyChar().nClass = GameScr.nClasss[msg.reader().readByte()];
				Char.getMyChar().sPoint = msg.reader().readShort();
				Char.getMyChar().pPoint = msg.reader().readShort();
				Char.getMyChar().vSkill.removeAllElements();
				Char.getMyChar().vSkillFight.removeAllElements();
				Char.getMyChar().myskill = null;
				break;
			case -125:
			{
				Char.getMyChar().readParam(msg, "Cmd.ME_LOAD_SKILL");
				if (Char.getMyChar().statusMe != 14 && Char.getMyChar().statusMe != 5)
				{
					Char.getMyChar().cHP = Char.getMyChar().cMaxHP;
					Char.getMyChar().cMP = Char.getMyChar().cMaxMP;
				}
				Char.getMyChar().sPoint = msg.reader().readShort();
				Char.getMyChar().vSkill.removeAllElements();
				Char.getMyChar().vSkillFight.removeAllElements();
				sbyte b6 = msg.reader().readByte();
				for (sbyte b7 = 0; b7 < b6; b7++)
				{
					Skill skill2 = Skills.get(msg.reader().readShort());
					if (Char.getMyChar().myskill == null)
					{
						Char.getMyChar().myskill = skill2;
					}
					else if (skill2.template.Equals(Char.getMyChar().myskill.template))
					{
						Char.getMyChar().myskill = skill2;
					}
					Char.getMyChar().vSkill.addElement(skill2);
					if ((skill2.template.type == 1 || skill2.template.type == 4 || skill2.template.type == 2 || skill2.template.type == 3) && (skill2.template.maxPoint == 0 || (skill2.template.maxPoint > 0 && skill2.point > 0)))
					{
						if (skill2.template.id == Char.getMyChar().skillTemplateId)
						{
							Service.gI().selectSkill(Char.getMyChar().skillTemplateId);
						}
						Char.getMyChar().vSkillFight.addElement(skill2);
					}
				}
				GameScr.gI().sortSkill();
				if (GameScr.isPaintInfoMe)
				{
					GameScr.indexRow = -1;
					GameScr.gI().setLCR();
				}
				break;
			}
			case -124:
				Char.getMyChar().readParam(msg, "Cmd.ME_LOAD_SKILL");
				Char.getMyChar().cEXP = msg.reader().readLong();
				GameScr.setLevel_Exp(Char.getMyChar().cEXP, value: true);
				Char.getMyChar().sPoint = msg.reader().readShort();
				Char.getMyChar().pPoint = msg.reader().readShort();
				Char.getMyChar().potential[0] = msg.reader().readShort();
				Char.getMyChar().potential[1] = msg.reader().readShort();
				Char.getMyChar().potential[2] = msg.reader().readInt();
				Char.getMyChar().potential[3] = msg.reader().readInt();
				break;
			case -123:
				Char.getMyChar().xu = msg.reader().readInt();
				Char.getMyChar().yen = msg.reader().readInt();
				Char.getMyChar().luong = msg.reader().readInt();
				Char.getMyChar().cHP = msg.reader().readInt();
				Char.getMyChar().cMP = msg.reader().readInt();
				if (msg.reader().readByte() == 1)
				{
					GameScr.gI().resetCaptcha();
					Char.getMyChar().isCaptcha = true;
				}
				else
				{
					Char.getMyChar().isCaptcha = false;
				}
				break;
			case -122:
				GameCanvas.debug("SA24", 2);
				Char.getMyChar().cHP = msg.reader().readInt();
				break;
			case -121:
				GameCanvas.debug("SA25", 2);
				Char.getMyChar().cMP = msg.reader().readInt();
				break;
			case -120:
			{
				Char char6 = GameScr.findCharInMap(msg.reader().readInt());
				if (char6 != null)
				{
					readCharInfo(char6, msg);
				}
				break;
			}
			case -119:
			{
				GameCanvas.debug("SA26", 2);
				Char char15 = GameScr.findCharInMap(msg.reader().readInt());
				if (char15 != null)
				{
					char15.cHP = msg.reader().readInt();
					char15.cMaxHP = msg.reader().readInt();
				}
				break;
			}
			case -117:
			{
				GameCanvas.debug("SA28", 2);
				Char char10 = GameScr.findCharInMap(msg.reader().readInt());
				if (char10 != null)
				{
					char10.cHP = msg.reader().readInt();
					char10.cMaxHP = msg.reader().readInt();
					char10.eff5BuffHp = msg.reader().readShort();
					char10.eff5BuffMp = msg.reader().readShort();
					char10.wp = msg.reader().readShort();
					if (char10.wp == -1)
					{
						char10.setDefaultWeapon();
					}
				}
				break;
			}
			case -116:
			{
				GameCanvas.debug("SA29", 2);
				Char char8 = GameScr.findCharInMap(msg.reader().readInt());
				if (char8 != null)
				{
					char8.cHP = msg.reader().readInt();
					char8.cMaxHP = msg.reader().readInt();
					char8.eff5BuffHp = msg.reader().readShort();
					char8.eff5BuffMp = msg.reader().readShort();
					char8.body = msg.reader().readShort();
					if (char8.body == -1)
					{
						char8.setDefaultBody();
					}
				}
				break;
			}
			case -113:
			{
				GameCanvas.debug("SA30", 2);
				Char char2 = GameScr.findCharInMap(msg.reader().readInt());
				if (char2 != null)
				{
					char2.cHP = msg.reader().readInt();
					char2.cMaxHP = msg.reader().readInt();
					char2.eff5BuffHp = msg.reader().readShort();
					char2.eff5BuffMp = msg.reader().readShort();
					char2.leg = msg.reader().readShort();
					if (char2.leg == -1)
					{
						char2.setDefaultLeg();
					}
				}
				break;
			}
			case -112:
			{
				GameCanvas.debug("SA31", 2);
				Char char7 = GameScr.findCharInMap(msg.reader().readInt());
				if (char7 != null)
				{
					char7.cHP = msg.reader().readInt();
					char7.cMaxHP = msg.reader().readInt();
					char7.eff5BuffHp = msg.reader().readShort();
					char7.eff5BuffMp = msg.reader().readShort();
				}
				break;
			}
			case -111:
			{
				GameCanvas.debug("SA32", 2);
				Char char19 = GameScr.findCharInMap(msg.reader().readInt());
				if (char19 != null)
				{
					char19.cHP = msg.reader().readInt();
				}
				break;
			}
			case -110:
			{
				GameCanvas.debug("SA33", 2);
				Char char17 = GameScr.findCharInMap(msg.reader().readInt());
				if (char17 != null)
				{
					char17.cHP = msg.reader().readInt();
					char17.cMaxHP = msg.reader().readInt();
					char17.cx = msg.reader().readShort();
					char17.cy = msg.reader().readShort();
					char17.statusMe = 1;
					ServerEffect.addServerEffect(20, char17, 2);
				}
				break;
			}
			case -109:
				Char.getMyChar().readParam(msg, "Cmd.ME_LOAD_SKILL");
				if (Char.getMyChar().statusMe != 14 && Char.getMyChar().statusMe != 5)
				{
					Char.getMyChar().cHP = Char.getMyChar().cMaxHP;
					Char.getMyChar().cMP = Char.getMyChar().cMaxMP;
				}
				Char.getMyChar().pPoint = msg.reader().readShort();
				Char.getMyChar().potential[0] = msg.reader().readShort();
				Char.getMyChar().potential[1] = msg.reader().readShort();
				Char.getMyChar().potential[2] = msg.reader().readInt();
				Char.getMyChar().potential[3] = msg.reader().readInt();
				break;
			case -107:
				GameCanvas.debug("SA16", 2);
				Char.getMyChar().bagSort();
				break;
			case -106:
				GameCanvas.debug("SA17", 2);
				Char.getMyChar().boxSort();
				break;
			case -105:
			{
				GameCanvas.debug("SA18", 2);
				int num26 = msg.reader().readInt();
				Char.getMyChar().xu -= num26;
				Char.getMyChar().xuInBox += num26;
				break;
			}
			case -104:
			{
				GameCanvas.debug("SA19", 2);
				int num17 = msg.reader().readInt();
				Char.getMyChar().xuInBox -= num17;
				Char.getMyChar().xu += num17;
				break;
			}
			case -102:
			{
				GameCanvas.debug("SA20", 2);
				Char.getMyChar().arrItemBag[msg.reader().readByte()] = null;
				Skill skill4 = Skills.get(msg.reader().readShort());
				Char.getMyChar().vSkill.addElement(skill4);
				if ((skill4.template.type == 1 || skill4.template.type == 4 || skill4.template.type == 2 || skill4.template.type == 3) && (skill4.template.maxPoint == 0 || (skill4.template.maxPoint > 0 && skill4.point > 0)))
				{
					if (skill4.template.id == Char.getMyChar().skillTemplateId)
					{
						Service.gI().selectSkill(Char.getMyChar().skillTemplateId);
					}
					Char.getMyChar().vSkillFight.addElement(skill4);
				}
				GameScr.gI().sortSkill();
				GameScr.gI().addSkillShortcut(skill4);
				GameScr.gI().setLCR();
				InfoMe.addInfo(mResources.LEARN_SKILL + " " + skill4.template.name);
				break;
			}
			case -101:
			{
				GameCanvas.debug("SA34", 2);
				Effect effect6 = new Effect(msg.reader().readByte(), (int)(mSystem.getCurrentTimeMillis() / 1000) - msg.reader().readInt(), msg.reader().readInt(), msg.reader().readShort());
				Char.getMyChar().vEff.addElement(effect6);
				if (effect6.template.type == 7)
				{
					Char.getMyChar().cMiss += effect6.param;
				}
				else if (effect6.template.type == 12 || effect6.template.type == 11)
				{
					Char.getMyChar().isInvisible = true;
					ServerEffect.addServerEffect(60, Char.getMyChar().cx, Char.getMyChar().cy, 1);
				}
				else if (effect6.template.type == 14)
				{
					GameCanvas.clearKeyPressed();
					GameCanvas.clearKeyHold();
					Char.getMyChar().cx = msg.reader().readShort();
					Char.getMyChar().cy = msg.reader().readShort();
					Char.getMyChar().statusMe = 1;
					Char.getMyChar().isLockMove = true;
					ServerEffect.addServerEffectWithTime(76, Char.getMyChar(), effect6.timeLenght);
				}
				else if (effect6.template.type == 1)
				{
					ServerEffect.addServerEffectWithTime(48, Char.getMyChar(), effect6.timeLenght);
				}
				else if (effect6.template.type == 2)
				{
					GameCanvas.clearKeyPressed();
					GameCanvas.clearKeyHold();
					Char.getMyChar().cx = msg.reader().readShort();
					Char.getMyChar().cy = msg.reader().readShort();
					Char.getMyChar().statusMe = 1;
					Char.getMyChar().isLockMove = true;
					Char.getMyChar().isLockAttack = true;
				}
				else if (effect6.template.type == 3)
				{
					GameCanvas.clearKeyPressed();
					GameCanvas.clearKeyHold();
					Char.getMyChar().cx = msg.reader().readShort();
					Char.getMyChar().cy = msg.reader().readShort();
					Char.getMyChar().statusMe = 1;
					Char.isLockKey = true;
					ServerEffect.addServerEffectWithTime(43, Char.getMyChar(), effect6.timeLenght);
				}
				break;
			}
			case -100:
			{
				GameCanvas.debug("SA35", 2);
				EffectTemplate effectTemplate = Effect.effTemplates[msg.reader().readByte()];
				for (int k = 0; k < Char.getMyChar().vEff.size(); k++)
				{
					Effect effect2 = (Effect)Char.getMyChar().vEff.elementAt(k);
					if (effect2 != null && effect2.template.type == effectTemplate.type)
					{
						if (effect2.template.type == 7)
						{
							Char.getMyChar().cMiss -= effect2.param;
						}
						effect2.template = effectTemplate;
						effect2.timeStart = (int)(mSystem.getCurrentTimeMillis() / 1000) - msg.reader().readInt();
						effect2.timeLenght = msg.reader().readInt() / 1000;
						effect2.param = msg.reader().readShort();
						if (effect2.template.type == 7)
						{
							Char.getMyChar().cMiss += effect2.param;
						}
						break;
					}
				}
				if (!GameScr.isPaintInfoMe)
				{
					GameScr.gI().resetButton();
				}
				break;
			}
			case -99:
			{
				GameCanvas.debug("SA36", 2);
				int num13 = msg.reader().readByte();
				Effect effect4 = null;
				for (int num14 = 0; num14 < Char.getMyChar().vEff.size(); num14++)
				{
					effect4 = (Effect)Char.getMyChar().vEff.elementAt(num14);
					if (effect4 != null && effect4.template.id == num13)
					{
						if (effect4.template.type == 7)
						{
							Char.getMyChar().cMiss -= effect4.param;
						}
						Char.getMyChar().vEff.removeElementAt(num14);
						break;
					}
				}
				if (effect4.template.type == 0 || effect4.template.type == 12)
				{
					Char.getMyChar().cHP = msg.reader().readInt();
					Char.getMyChar().cMP = msg.reader().readInt();
					if (effect4.template.type == 0)
					{
						InfoMe.addInfo(mResources.EFF_REMOVE);
					}
					else if (effect4.template.type == 12)
					{
						Char.getMyChar().isInvisible = false;
						ServerEffect.addServerEffect(60, Char.getMyChar().cx, Char.getMyChar().cy, 1);
					}
				}
				else if (effect4.template.type == 4 || effect4.template.type == 13 || effect4.template.type == 17)
				{
					Char.getMyChar().cHP = msg.reader().readInt();
				}
				else if (effect4.template.type == 23)
				{
					Char.getMyChar().cHP = msg.reader().readInt();
					Char.getMyChar().cMaxHP = msg.reader().readInt();
				}
				else if (effect4.template.type == 11)
				{
					Char.getMyChar().isInvisible = false;
					ServerEffect.addServerEffect(60, Char.getMyChar().cx, Char.getMyChar().cy, 1);
				}
				else if (effect4.template.type == 14)
				{
					Char.getMyChar().isLockMove = false;
				}
				else if (effect4.template.type == 2)
				{
					Char.getMyChar().isLockMove = false;
					Char.getMyChar().isLockAttack = false;
					ServerEffect.addServerEffect(77, Char.getMyChar().cx, Char.getMyChar().cy - 9, 1);
				}
				else if (effect4.template.type == 3)
				{
					Char.isLockKey = false;
				}
				break;
			}
			case -98:
				GameCanvas.debug("SA344", 2);
				try
				{
					Char char13 = GameScr.findCharInMap(msg.reader().readInt());
					if (char13 != null)
					{
						Effect effect5 = new Effect(msg.reader().readByte(), (int)(mSystem.getCurrentTimeMillis() / 1000) - msg.reader().readInt(), msg.reader().readInt(), msg.reader().readShort());
						char13.vEff.addElement(effect5);
						if (effect5.template.type == 12 || effect5.template.type == 11)
						{
							char13.isInvisible = true;
							ServerEffect.addServerEffect(60, char13.cx, char13.cy, 1);
						}
						else if (effect5.template.type == 14)
						{
							char13.cx = (char13.cxMoveLast = msg.reader().readShort());
							char13.cy = (char13.cyMoveLast = msg.reader().readShort());
							char13.statusMe = 1;
							ServerEffect.addServerEffectWithTime(76, char13, effect5.timeLenght);
						}
						else if (effect5.template.type == 1)
						{
							ServerEffect.addServerEffectWithTime(48, char13, effect5.timeLenght);
						}
						else if (effect5.template.type == 2)
						{
							char13.cx = (char13.cxMoveLast = msg.reader().readShort());
							char13.cy = (char13.cyMoveLast = msg.reader().readShort());
							char13.statusMe = 1;
							char13.isLockAttack = true;
						}
						else if (effect5.template.type == 3)
						{
							char13.cx = (char13.cxMoveLast = msg.reader().readShort());
							char13.cy = (char13.cyMoveLast = msg.reader().readShort());
							char13.statusMe = 1;
							ServerEffect.addServerEffectWithTime(43, char13, effect5.timeLenght);
						}
					}
					break;
				}
				catch (Exception)
				{
					break;
				}
			case -97:
				GameCanvas.debug("SA355", 2);
				try
				{
					Char char12 = GameScr.findCharInMap(msg.reader().readInt());
					if (char12 == null)
					{
						break;
					}
					EffectTemplate effectTemplate2 = Effect.effTemplates[msg.reader().readByte()];
					for (int m = 0; m < char12.vEff.size(); m++)
					{
						Effect effect3 = (Effect)char12.vEff.elementAt(m);
						if (effect3 != null && effectTemplate2.type == effectTemplate2.type)
						{
							effect3.template = effectTemplate2;
							effect3.timeStart = (int)(mSystem.getCurrentTimeMillis() / 1000) - msg.reader().readInt();
							effect3.timeLenght = msg.reader().readInt() / 1000;
							effect3.param = msg.reader().readShort();
							break;
						}
					}
					break;
				}
				catch (Exception)
				{
					break;
				}
			case -92:
			{
				GameCanvas.debug("SY3", 2);
				int num4 = msg.reader().readInt();
				Char char9 = ((num4 != Char.getMyChar().charID) ? GameScr.findCharInMap(num4) : Char.getMyChar());
				if (char9 != null)
				{
					char9.cTypePk = msg.reader().readByte();
				}
				break;
			}
			case -91:
			{
				GameCanvas.debug("SY6", 2);
				Item[] array = new Item[msg.reader().readUnsignedByte()];
				for (int l = 0; l < Char.getMyChar().arrItemBag.Length; l++)
				{
					array[l] = Char.getMyChar().arrItemBag[l];
				}
				Char.getMyChar().arrItemBag = array;
				Char.getMyChar().arrItemBag[msg.reader().readUnsignedByte()] = null;
				InfoMe.addInfo(mResources.BAG_EXPANDED + " " + Char.getMyChar().arrItemBag.Length + " ô");
				break;
			}
			case -90:
			{
				GameCanvas.debug("SY7", 2);
				for (int j = 0; j < GameScr.vNpc.size(); j++)
				{
					Npc npc = (Npc)GameScr.vNpc.elementAt(j);
					if (npc != null && npc.statusMe == 15)
					{
						npc.statusMe = 1;
						break;
					}
				}
				switch (msg.reader().readByte())
				{
				case 1:
					InfoMe.addInfo(mResources.PROTECT_FAR, 20, mFont.tahoma_7_yellow);
					break;
				case 2:
					InfoMe.addInfo(mResources.PROTECT_INJURE, 20, mFont.tahoma_7_yellow);
					break;
				}
				break;
			}
			case -89:
				GameCanvas.isLoading = false;
				GameCanvas.debug("SY8", 2);
				try
				{
					InfoMe.addInfo(msg.reader().readUTF(), 20, mFont.tahoma_7_yellow);
				}
				catch (Exception)
				{
				}
				InfoDlg.hide();
				GameCanvas.endDlg();
				break;
			case -87:
			{
				GameCanvas.debug("SY9", 2);
				int index2 = msg.reader().readByte();
				Party party = (Party)GameScr.vParty.elementAt(index2);
				GameScr.vParty.setElementAt(GameScr.vParty.elementAt(0), index2);
				if (party != null)
				{
					GameScr.vParty.setElementAt(party, 0);
				}
				GameScr.gI().refreshTeam();
				if (party != null)
				{
					InfoMe.addInfo(party.name + mResources.TEAMLEADER_CHANGE, 20, mFont.tahoma_7_yellow);
				}
				break;
			}
			case -86:
				GameCanvas.debug("SYA1", 2);
				GameScr.vParty.removeAllElements();
				GameScr.gI().refreshTeam();
				InfoMe.addInfo(mResources.MOVEOUT_ME, 20, mFont.tahoma_7_yellow);
				break;
			case -85:
			{
				GameCanvas.debug("SYA2", 2);
				GameScr.vFriend.removeAllElements();
				try
				{
					while (true)
					{
						GameScr.vFriend.addElement(new Friend(msg.reader().readUTF(), msg.reader().readByte()));
					}
				}
				catch (Exception)
				{
				}
				for (int num25 = 0; num25 < GameScr.vFriendWait.size(); num25++)
				{
					GameScr.vFriend.addElement(GameScr.vFriendWait.elementAt(num25));
				}
				GameScr.gI().sortList(0);
				break;
			}
			case -84:
				GameCanvas.debug("SYA3", 2);
				GameScr.vEnemies.removeAllElements();
				try
				{
					while (true)
					{
						GameScr.vEnemies.addElement(new Friend(msg.reader().readUTF(), msg.reader().readByte()));
					}
				}
				catch (Exception)
				{
				}
				GameScr.gI().sortList(1);
				break;
			case -83:
			{
				GameCanvas.debug("SYA4", 2);
				string text2 = msg.reader().readUTF();
				for (int num19 = 0; num19 < GameScr.vFriend.size(); num19++)
				{
					Friend friend2 = (Friend)GameScr.vFriend.elementAt(num19);
					if (friend2 != null && friend2.friendName.Equals(text2))
					{
						GameScr.indexRow = 0;
						GameScr.vFriend.removeElementAt(num19);
						GameScr.gI().actRemoveWaitAcceptFriend(text2);
						break;
					}
				}
				if (GameScr.isPaintFriend)
				{
					GameScr.gI().sortList(0);
					GameScr.indexRow = 0;
					GameScr.scrMain.clear();
				}
				break;
			}
			case -82:
			{
				GameCanvas.debug("SYA5", 2);
				string value = msg.reader().readUTF();
				for (int num18 = 0; num18 < GameScr.vEnemies.size(); num18++)
				{
					Friend friend = (Friend)GameScr.vEnemies.elementAt(num18);
					if (friend != null && friend.friendName.Equals(value))
					{
						GameScr.indexRow = 0;
						GameScr.vEnemies.removeElementAt(num18);
						break;
					}
				}
				GameScr.gI().sortList(0);
				break;
			}
			case -81:
				GameCanvas.debug("SYA6", 2);
				Char.getMyChar().cPk = msg.reader().readByte();
				Char.getMyChar().charFocus = null;
				break;
			case -80:
				Char.getMyChar().arrItemBody[msg.reader().readByte()] = null;
				break;
			case -78:
				GameCanvas.debug("SY4", 2);
				ServerEffect.addServerEffect(msg.reader().readShort(), Char.getMyChar().cx, Char.getMyChar().cy, 1);
				break;
			case -77:
				GameCanvas.debug("SY5", 2);
				try
				{
					GameScr.vPtMap.removeAllElements();
					while (true)
					{
						GameScr.vPtMap.addElement(new Party(msg.reader().readByte(), msg.reader().readUnsignedByte(), msg.reader().readUTF(), msg.reader().readByte()));
					}
				}
				catch (Exception)
				{
				}
				GameScr.gI().refreshFindTeam();
				break;
			case -76:
				((Party)GameScr.vParty.firstElement()).isLock = msg.reader().readBoolean();
				break;
			case -75:
				Char.getMyChar().arrItemBox[msg.reader().readByte()] = null;
				break;
			case -74:
				InfoDlg.showWait(msg.reader().readUTF());
				break;
			case -73:
			{
				GameCanvas.debug("SY4", 2);
				Mob mob = Mob.get_Mob(msg.reader().readUnsignedByte());
				ServerEffect.addServerEffect(67, mob.x, mob.y, 1);
				break;
			}
			case -72:
				GameCanvas.debug("SY4", 2);
				Char.getMyChar().luong = msg.reader().readInt();
				break;
			case -71:
			{
				GameCanvas.debug("SY422", 2);
				int num16 = msg.reader().readInt();
				Char.getMyChar().luong += num16;
				GameScr.startFlyText("+" + num16, Char.getMyChar().cx, Char.getMyChar().cy - Char.getMyChar().ch - 10, 0, -2, mFont.ADDMONEY);
				InfoMe.addInfo(mResources.RECEIVE + " " + num16 + " " + mResources.GOLD, 20, mFont.tahoma_7_yellow);
				break;
			}
			case -69:
			{
				GameCanvas.debug("SY42222EE", 2);
				int num15 = msg.reader().readUnsignedByte();
				sbyte b5 = msg.reader().readByte();
				if (num15 > 0)
				{
					short pointx2 = (short)Char.getMyChar().cx;
					short pointy2 = (short)(Char.getMyChar().cy - 40);
					Char.getMyChar().mobMe = new Mob(-1, isDisable: false, isDontMove: false, isFire: false, isIce: false, isWind: false, num15, 1, 0, 0, 0, pointx2, pointy2, 4, 0, b5 != 0, removeWhenDie: false);
					Char.getMyChar().mobMe.status = 5;
				}
				else
				{
					Char.getMyChar().mobMe = null;
				}
				break;
			}
			case -68:
			{
				GameCanvas.debug("SY42222E", 2);
				Char char11 = GameScr.findCharInMap(msg.reader().readInt());
				if (char11 != null)
				{
					int num5 = msg.reader().readUnsignedByte();
					sbyte b2 = msg.reader().readByte();
					if (num5 > 0)
					{
						short pointx = (short)char11.cx;
						short pointy = (short)(char11.cy - 40);
						char11.mobMe = new Mob(-1, isDisable: false, isDontMove: false, isFire: false, isIce: false, isWind: false, num5, 1, 0, 0, 0, pointx, pointy, 4, 0, b2 != 0, removeWhenDie: false);
						char11.mobMe.status = 5;
					}
					else
					{
						char11.mobMe = null;
					}
				}
				break;
			}
			case -65:
			{
				string text = msg.reader().readUTF();
				sbyte[] data = new sbyte[msg.reader().readInt()];
				msg.reader().read(ref data);
				if (data.Length == 0)
				{
					data = null;
				}
				try
				{
					msg.reader().readByte();
				}
				catch (Exception)
				{
				}
				if (text.Equals("KSkill"))
				{
					GameScr.gI().onKSkill(data);
				}
				else if (text.Equals("OSkill"))
				{
					GameScr.gI().onOSkill(data);
				}
				else if (text.Equals("CSkill"))
				{
					GameScr.gI().onCSkill(data);
				}
				break;
			}
			case -64:
			{
				GameCanvas.debug("SA30", 2);
				Char char5 = GameScr.findCharInMap(msg.reader().readInt());
				if (char5 != null)
				{
					char5.cHP = msg.reader().readInt();
					char5.cMaxHP = msg.reader().readInt();
					char5.eff5BuffHp = msg.reader().readShort();
					char5.eff5BuffMp = msg.reader().readShort();
					char5.head = msg.reader().readShort();
				}
				break;
			}
			case -63:
			{
				GameCanvas.debug("SA3001", 2);
				int num3 = msg.reader().readInt();
				Char char4 = GameScr.findCharInMap(num3);
				if (char4 != null)
				{
					GameCanvas.startYesNoDlg(char4.cName + " " + mResources.replace(mResources.INVITECLAN, msg.reader().readUTF()), 88830, num3, 88811, null);
				}
				break;
			}
			case -62:
			{
				GameCanvas.debug("SA3001", 2);
				int num2 = msg.reader().readInt();
				string cClanName = msg.reader().readUTF();
				sbyte ctypeClan = msg.reader().readByte();
				if (Char.getMyChar().charID == num2)
				{
					Char.getMyChar().cClanName = cClanName;
					Char.getMyChar().ctypeClan = ctypeClan;
					Char.getMyChar().callEffTask(21);
					break;
				}
				Char char3 = GameScr.findCharInMap(num2);
				if (char3 != null)
				{
					char3.cClanName = cClanName;
					char3.ctypeClan = ctypeClan;
				}
				break;
			}
			case -61:
			{
				GameCanvas.debug("SA30021", 2);
				int num36 = msg.reader().readInt();
				if (GameScr.isViewClanInvite)
				{
					int num37 = num36;
					Char char21 = GameScr.findCharInMap(num37);
					if (char21 != null)
					{
						GameCanvas.startYesNoDlg(char21.cName + " " + mResources.PLEASECLAN, 88831, num37, 88811, null);
					}
				}
				break;
			}
			case -59:
			{
				int num27 = msg.reader().readInt();
				Char obj = ((num27 != Char.getMyChar().charID) ? GameScr.findCharInMap(num27) : Char.getMyChar());
				obj.cHP = msg.reader().readInt();
				obj.cMaxHP = msg.reader().readInt();
				break;
			}
			case -58:
				GameScr.gI().resetButton();
				GameCanvas.timeBallEffect = 70;
				GameCanvas.isBallEffect = true;
				ServerEffect.addServerEffect(119, GameScr.gW2 + GameScr.cmx, GameScr.gH2 + GameScr.cmy, 1);
				break;
			case -57:
				GameCanvas.timeBallEffect = 40;
				GameCanvas.isBallEffect = true;
				break;
			case -56:
			{
				Char char20 = GameScr.findCharInMap(msg.reader().readInt());
				if (char20 != null)
				{
					char20.cHP = msg.reader().readInt();
					char20.cMaxHP = msg.reader().readInt();
					char20.coat = (short)msg.reader().readUnsignedShort();
				}
				break;
			}
			case -55:
			{
				Char char18 = GameScr.findCharInMap(msg.reader().readInt());
				if (char18 != null)
				{
					char18.cHP = msg.reader().readInt();
					char18.cMaxHP = msg.reader().readInt();
					char18.glove = (short)msg.reader().readUnsignedShort();
				}
				break;
			}
			case -54:
			{
				int num20 = msg.reader().readInt();
				Char char16 = ((Char.getMyChar().charID != num20) ? GameScr.findCharInMap(num20) : Char.getMyChar());
				if (char16 == null)
				{
					break;
				}
				char16.arrItemMounts = new Item[5];
				char16.isNewMount = (char16.isWolf = (char16.isMoto = (char16.isMotoBehind = false)));
				for (int num21 = 0; num21 < char16.arrItemMounts.Length; num21++)
				{
					short num22 = msg.reader().readShort();
					if (num22 == -1)
					{
						continue;
					}
					char16.arrItemMounts[num21] = new Item();
					char16.arrItemMounts[num21].typeUI = 41;
					char16.arrItemMounts[num21].indexUI = num21;
					char16.arrItemMounts[num21].template = ItemTemplates.get(num22);
					char16.arrItemMounts[num21].upgrade = msg.reader().readByte();
					char16.arrItemMounts[num21].expires = msg.reader().readLong();
					char16.arrItemMounts[num21].sys = msg.reader().readByte();
					char16.arrItemMounts[num21].isLock = true;
					if (num21 == 4)
					{
						if (char16.arrItemMounts[num21].template.id == 485 || char16.arrItemMounts[num21].template.id == 524)
						{
							char16.isMoto = true;
						}
						else if (char16.arrItemMounts[num21].template.id == 443 || char16.arrItemMounts[num21].template.id == 523)
						{
							char16.isWolf = true;
						}
						else
						{
							char16.isNewMount = true;
							char16.GetNewMount();
						}
					}
					int num23 = msg.reader().readByte();
					char16.arrItemMounts[num21].options = new MyVector();
					for (int num24 = 0; num24 < num23; num24++)
					{
						char16.arrItemMounts[num21].options.addElement(new ItemOption(msg.reader().readUnsignedByte(), msg.reader().readInt()));
					}
				}
				break;
			}
			case 115:
			{
				GameScr.currentCharViewInfo = Char.getMyChar();
				Char.getMyChar().charID = msg.reader().readInt();
				Char.getMyChar().cClanName = msg.reader().readUTF();
				if (!Char.getMyChar().cClanName.Equals(string.Empty))
				{
					Char.getMyChar().ctypeClan = msg.reader().readByte();
				}
				Char.getMyChar().ctaskId = msg.reader().readByte();
				Char.getMyChar().cgender = msg.reader().readByte();
				Char.getMyChar().head = msg.reader().readShort();
				Char.getMyChar().cspeed = msg.reader().readByte();
				Char.getMyChar().cName = msg.reader().readUTF();
				Char.getMyChar().cPk = msg.reader().readByte();
				Char.getMyChar().cTypePk = msg.reader().readByte();
				Char.getMyChar().cMaxHP = msg.reader().readInt();
				Char.getMyChar().cHP = msg.reader().readInt();
				Char.getMyChar().cMaxMP = msg.reader().readInt();
				Char.getMyChar().cMP = msg.reader().readInt();
				Char.getMyChar().cEXP = msg.reader().readLong();
				Char.getMyChar().cExpDown = msg.reader().readLong();
				GameScr.setLevel_Exp(Char.getMyChar().cEXP, value: true);
				Char.getMyChar().eff5BuffHp = msg.reader().readShort();
				Char.getMyChar().eff5BuffMp = msg.reader().readShort();
				Char.getMyChar().nClass = GameScr.nClasss[msg.reader().readByte()];
				Char.getMyChar().pPoint = msg.reader().readShort();
				Char.getMyChar().potential[0] = msg.reader().readShort();
				Char.getMyChar().potential[1] = msg.reader().readShort();
				Char.getMyChar().potential[2] = msg.reader().readInt();
				Char.getMyChar().potential[3] = msg.reader().readInt();
				Char.getMyChar().sPoint = msg.reader().readShort();
				Char.getMyChar().vSkill.removeAllElements();
				Char.getMyChar().vSkillFight.removeAllElements();
				sbyte b3 = msg.reader().readByte();
				for (byte b4 = 0; b4 < (byte)b3; b4++)
				{
					Skill skill = Skills.get(msg.reader().readShort());
					if (Char.getMyChar().myskill == null)
					{
						Char.getMyChar().myskill = skill;
					}
					Char.getMyChar().vSkill.addElement(skill);
					if ((skill.template.type == 1 || skill.template.type == 4 || skill.template.type == 2 || skill.template.type == 3) && (skill.template.maxPoint == 0 || (skill.template.maxPoint > 0 && skill.point > 0)))
					{
						if (skill.template.id == Char.getMyChar().skillTemplateId)
						{
							Service.gI().selectSkill(Char.getMyChar().skillTemplateId);
						}
						Char.getMyChar().vSkillFight.addElement(skill);
					}
				}
				GameScr.gI().sortSkill();
				Char.getMyChar().xu = msg.reader().readInt();
				Char.getMyChar().yen = msg.reader().readInt();
				Char.getMyChar().luong = msg.reader().readInt();
				Char.getMyChar().arrItemBag = new Item[msg.reader().readUnsignedByte()];
				GameScr.hpPotion = (GameScr.mpPotion = 0);
				for (int n = 0; n < Char.getMyChar().arrItemBag.Length; n++)
				{
					short num6 = msg.reader().readShort();
					if (num6 != -1)
					{
						Char.getMyChar().arrItemBag[n] = new Item();
						Char.getMyChar().arrItemBag[n].typeUI = 3;
						Char.getMyChar().arrItemBag[n].indexUI = n;
						Char.getMyChar().arrItemBag[n].template = ItemTemplates.get(num6);
						Char.getMyChar().arrItemBag[n].isLock = msg.reader().readBoolean();
						if (Char.getMyChar().arrItemBag[n].isTypeBody() || Char.getMyChar().arrItemBag[n].isTypeMounts() || Char.getMyChar().arrItemBag[n].isTypeNgocKham())
						{
							Char.getMyChar().arrItemBag[n].upgrade = msg.reader().readByte();
						}
						Char.getMyChar().arrItemBag[n].isExpires = msg.reader().readBoolean();
						Char.getMyChar().arrItemBag[n].quantity = msg.reader().readUnsignedShort();
						if (Char.getMyChar().arrItemBag[n].template.type == 16)
						{
							GameScr.hpPotion += Char.getMyChar().arrItemBag[n].quantity;
						}
						if (Char.getMyChar().arrItemBag[n].template.type == 17)
						{
							GameScr.mpPotion += Char.getMyChar().arrItemBag[n].quantity;
						}
						if (Char.getMyChar().arrItemBag[n].template.id == 340)
						{
							GameScr.gI().numSprinLeft += Char.getMyChar().arrItemBag[n].quantity;
						}
					}
				}
				Char.getMyChar().arrItemBody = new Item[32];
				try
				{
					Char.getMyChar().setDefaultPart();
					for (int num7 = 0; num7 < 16; num7++)
					{
						short num8 = msg.reader().readShort();
						if (num8 != -1)
						{
							ItemTemplate itemTemplate = ItemTemplates.get(num8);
							int type = itemTemplate.type;
							Char.getMyChar().arrItemBody[type] = new Item();
							Char.getMyChar().arrItemBody[type].indexUI = type;
							Char.getMyChar().arrItemBody[type].typeUI = 5;
							Char.getMyChar().arrItemBody[type].template = itemTemplate;
							Char.getMyChar().arrItemBody[type].isLock = true;
							Char.getMyChar().arrItemBody[type].upgrade = msg.reader().readByte();
							Char.getMyChar().arrItemBody[type].sys = msg.reader().readByte();
							switch (type)
							{
							case 6:
								Char.getMyChar().leg = Char.getMyChar().arrItemBody[type].template.part;
								break;
							case 2:
								Char.getMyChar().body = Char.getMyChar().arrItemBody[type].template.part;
								break;
							case 1:
								Char.getMyChar().wp = Char.getMyChar().arrItemBody[type].template.part;
								break;
							}
						}
					}
				}
				catch (Exception)
				{
				}
				Char.getMyChar().isHuman = msg.reader().readBoolean();
				Char.getMyChar().isNhanban = msg.reader().readBoolean();
				short[] array2 = new short[4]
				{
					msg.reader().readShort(),
					msg.reader().readShort(),
					msg.reader().readShort(),
					msg.reader().readShort()
				};
				if (array2[0] > -1)
				{
					Char.getMyChar().head = array2[0];
				}
				if (array2[1] > -1)
				{
					Char.getMyChar().wp = array2[1];
				}
				if (array2[2] > -1)
				{
					Char.getMyChar().body = array2[2];
				}
				if (array2[3] > -1)
				{
					Char.getMyChar().leg = array2[3];
				}
				short[] array3 = new short[10];
				try
				{
					for (int num9 = 0; num9 < 10; num9++)
					{
						array3[num9] = msg.reader().readShort();
					}
				}
				catch (Exception)
				{
					array3 = null;
				}
				if (array3 != null)
				{
					Char.getMyChar().setThoiTrang(array3);
				}
				GameScr.gI().sortSkill();
				if (Char.getMyChar().isHuman)
				{
					GameScr.gI().loadSkillShortcut();
				}
				else if (Char.getMyChar().isNhanban)
				{
					GameScr.gI().loadSkillShortcutNhanban();
				}
				Char.getMyChar().statusMe = 4;
				GameScr.isViewClanInvite = RMS.loadRMSInt(Char.getMyChar().cName + "vci") >= 1;
				Service.gI().loadRMS("KSkill");
				Service.gI().loadRMS("OSkill");
				Service.gI().loadRMS("CSkill");
				try
				{
					for (int num10 = 0; num10 < 16; num10++)
					{
						short num11 = msg.reader().readShort();
						if (num11 != -1)
						{
							ItemTemplate itemTemplate2 = ItemTemplates.get(num11);
							int num12 = itemTemplate2.type + 16;
							Char.getMyChar().arrItemBody[num12] = new Item();
							Char.getMyChar().arrItemBody[num12].indexUI = num12;
							Char.getMyChar().arrItemBody[num12].typeUI = 5;
							Char.getMyChar().arrItemBody[num12].template = itemTemplate2;
							Char.getMyChar().arrItemBody[num12].isLock = true;
							Char.getMyChar().arrItemBody[num12].upgrade = msg.reader().readByte();
							Char.getMyChar().arrItemBody[num12].sys = msg.reader().readByte();
							switch (num12)
							{
							case 6:
								Char.getMyChar().leg = Char.getMyChar().arrItemBody[num12].template.part;
								break;
							case 2:
								Char.getMyChar().body = Char.getMyChar().arrItemBody[num12].template.part;
								break;
							case 1:
								Char.getMyChar().wp = Char.getMyChar().arrItemBody[num12].template.part;
								break;
							}
						}
					}
					break;
				}
				catch (Exception)
				{
					break;
				}
			}
			case -94:
			{
				GameCanvas.debug("SY1", 2);
				int index = msg.reader().readByte();
				Npc npc2 = (Npc)GameScr.vNpc.elementAt(index);
				npc2.statusMe = msg.reader().readByte();
				if (npc2.template.npcTemplateId == 31 && npc2.statusMe == 15)
				{
					GameScr.startLanterns(npc2.cx, npc2.cy);
				}
				break;
			}
			case -95:
				GameCanvas.debug("SXX9", 2);
				GameScr.gI().timeLengthMap = msg.reader().readInt();
				GameScr.gI().timeStartMap = (int)(mSystem.getCurrentTimeMillis() / 1000);
				break;
			case -96:
			{
				GameCanvas.debug("SA366", 2);
				Char @char = GameScr.findCharInMap(msg.reader().readInt());
				GameCanvas.debug("SA366x1", 2);
				if (@char == null)
				{
					break;
				}
				GameCanvas.debug("SA366x2", 2);
				int num = msg.reader().readByte();
				Effect effect = null;
				for (int i = 0; i < @char.vEff.size(); i++)
				{
					GameCanvas.debug("SA366x3k" + i, 2);
					effect = (Effect)@char.vEff.elementAt(i);
					if (effect != null)
					{
						if (effect.template.id == num)
						{
							@char.vEff.removeElementAt(i);
							break;
						}
						GameCanvas.debug("SA366x3i" + i, 2);
					}
				}
				GameCanvas.debug("SA366x5", 2);
				if (effect != null)
				{
					if (effect.template.type == 0)
					{
						GameCanvas.debug("SA366x5a2", 2);
						@char.cHP = msg.reader().readInt();
						@char.cMP = msg.reader().readInt();
					}
					else if (effect.template.type == 11)
					{
						@char.cx = (@char.cxMoveLast = msg.reader().readUnsignedShort());
						@char.cy = (@char.cyMoveLast = msg.reader().readUnsignedShort());
						@char.isInvisible = false;
						ServerEffect.addServerEffect(60, @char.cx, @char.cy, 1);
					}
					else if (effect.template.type == 12)
					{
						@char.cHP = msg.reader().readInt();
						@char.cMP = msg.reader().readInt();
						@char.isInvisible = false;
						ServerEffect.addServerEffect(60, @char.cx, @char.cy, 1);
					}
					else if (effect.template.type == 4 || effect.template.type == 13 || effect.template.type == 17)
					{
						@char.cHP = msg.reader().readInt();
					}
					else if (effect.template.type == 23)
					{
						Char.getMyChar().cHP = msg.reader().readInt();
						Char.getMyChar().cMaxHP = msg.reader().readInt();
					}
					else if (effect.template.type == 2)
					{
						@char.isLockAttack = false;
						ServerEffect.addServerEffect(77, @char.cx, @char.cy - 9, 1);
					}
				}
				GameCanvas.debug("SA366x7", 2);
				break;
			}
			}
		}
		catch (Exception)
		{
		}
		finally
		{
			msg?.cleanup();
		}
	}

	public bool readCharInfo(Char c, Message msg)
	{
		try
		{
			c.cClanName = msg.reader().readUTF();
			if (!c.cClanName.Equals(string.Empty))
			{
				c.ctypeClan = msg.reader().readByte();
			}
			c.isInvisible = msg.reader().readBoolean();
			c.cTypePk = msg.reader().readByte();
			c.nClass = GameScr.nClasss[msg.reader().readByte()];
			c.cgender = msg.reader().readByte();
			c.head = msg.reader().readShort();
			c.cName = msg.reader().readUTF();
			c.cHP = msg.reader().readInt();
			c.cMaxHP = msg.reader().readInt();
			c.clevel = msg.reader().readUnsignedByte();
			c.wp = msg.reader().readShort();
			c.body = msg.reader().readShort();
			c.leg = msg.reader().readShort();
			sbyte b = msg.reader().readByte();
			if (c.wp == -1)
			{
				c.setDefaultWeapon();
			}
			if (c.body == -1)
			{
				c.setDefaultBody();
			}
			if (c.leg == -1)
			{
				c.setDefaultLeg();
			}
			if (b == -1)
			{
				c.mobMe = null;
			}
			else
			{
				short pointx = (short)c.cx;
				short pointy = (short)(c.cy - 40);
				c.mobMe = new Mob(-1, isDisable: false, isDontMove: false, isFire: false, isIce: false, isWind: false, b, 1, 0, 0, 0, pointx, pointy, 4, 0, isBos: false, removeWhenDie: false);
				c.mobMe.status = 5;
			}
			c.cx = (c.cxMoveLast = msg.reader().readShort());
			c.cy = (c.cyMoveLast = msg.reader().readShort());
			c.eff5BuffHp = msg.reader().readShort();
			c.eff5BuffMp = msg.reader().readShort();
			int num = msg.reader().readByte();
			for (int i = 0; i < num; i++)
			{
				Effect effect = new Effect(msg.reader().readByte(), msg.reader().readInt(), msg.reader().readInt(), msg.reader().readShort());
				c.vEff.addElement(effect);
				if (effect.template.type == 12 || effect.template.type == 11)
				{
					c.isInvisible = true;
				}
			}
			if (!c.isInvisible)
			{
				ServerEffect.addServerEffect(60, c, 1);
			}
			if (c.cHP == 0)
			{
				c.statusMe = 14;
				if (Char.getMyChar().charID == c.charID)
				{
					GameScr.gI().resetButton();
				}
			}
			if (c.charID == -Char.getMyChar().charID)
			{
				for (int j = 0; j < GameScr.vNpc.size(); j++)
				{
					Npc npc = (Npc)GameScr.vNpc.elementAt(j);
					if (npc.template.name.Equals(c.cName))
					{
						npc.hide();
						break;
					}
				}
			}
			c.isHuman = msg.reader().readBoolean();
			c.isNhanban = msg.reader().readBoolean();
			if (c.isNhanbanz())
			{
				ServerEffect.addServerEffect(141, c.cx, c.cy, 0);
			}
			short[] array = new short[4]
			{
				msg.reader().readShort(),
				msg.reader().readShort(),
				msg.reader().readShort(),
				msg.reader().readShort()
			};
			if (array[0] > -1)
			{
				c.head = array[0];
			}
			if (array[1] > -1)
			{
				c.wp = array[1];
			}
			if (array[2] > -1)
			{
				c.body = array[2];
			}
			if (array[3] > -1)
			{
				c.leg = array[3];
			}
			short[] array2 = new short[10];
			try
			{
				for (int k = 0; k < 10; k++)
				{
					array2[k] = msg.reader().readShort();
				}
			}
			catch (Exception)
			{
			}
			if (array2 != null)
			{
				c.setThoiTrang(array2);
			}
			Party.refresh(c);
			return true;
		}
		catch (Exception)
		{
		}
		return false;
	}

	public void requestItemInfo(Message msg)
	{
		try
		{
			int num = msg.reader().readByte();
			int num2 = msg.reader().readUnsignedByte();
			Item item = null;
			switch (num)
			{
			case 2:
				item = GameScr.arrItemWeapon[num2];
				break;
			case 3:
			{
				item = Char.getMyChar().arrItemBag[num2];
				if (item != null)
				{
					break;
				}
				if (GameScr.itemSplit != null && GameScr.itemSplit.indexUI == num2)
				{
					item = GameScr.itemSplit;
				}
				if (GameScr.itemUpGrade != null && GameScr.itemUpGrade.indexUI == num2)
				{
					item = GameScr.itemUpGrade;
				}
				if (GameScr.itemSell != null && GameScr.itemSell.indexUI == num2)
				{
					item = GameScr.itemSell;
				}
				if (item == null && GameScr.arrItemUpGrade != null)
				{
					for (int i = 0; i < GameScr.arrItemUpGrade.Length; i++)
					{
						if (GameScr.arrItemUpGrade[i] != null && GameScr.arrItemUpGrade[i].indexUI == num2)
						{
							item = GameScr.arrItemUpGrade[i];
							break;
						}
					}
				}
				if (item == null && GameScr.arrItemConvert != null)
				{
					for (int j = 0; j < GameScr.arrItemConvert.Length; j++)
					{
						if (GameScr.arrItemConvert[j] != null && GameScr.arrItemConvert[j].indexUI == num2)
						{
							item = GameScr.arrItemConvert[j];
							break;
						}
					}
				}
				if (item == null && GameScr.arrItemUpPeal != null)
				{
					for (int k = 0; k < GameScr.arrItemUpPeal.Length; k++)
					{
						if (GameScr.arrItemUpPeal[k] != null && GameScr.arrItemUpPeal[k].indexUI == num2)
						{
							item = GameScr.arrItemUpPeal[k];
							break;
						}
					}
				}
				if (item == null && GameScr.arrItemTradeMe != null)
				{
					for (int l = 0; l < GameScr.arrItemTradeMe.Length; l++)
					{
						if (GameScr.arrItemTradeMe[l] != null && GameScr.arrItemTradeMe[l].indexUI == num2)
						{
							item = GameScr.arrItemTradeMe[l];
							break;
						}
					}
				}
				if (item != null || GameScr.arrItemSplit == null)
				{
					break;
				}
				for (int m = 0; m < GameScr.arrItemSplit.Length; m++)
				{
					if (GameScr.arrItemSplit[m] != null && GameScr.arrItemSplit[m].indexUI == num2)
					{
						item = GameScr.arrItemSplit[m];
						break;
					}
				}
				break;
			}
			case 4:
				item = Char.getMyChar().arrItemBox[num2];
				break;
			case 5:
				item = Char.getMyChar().arrItemBody[num2];
				break;
			case 6:
				item = GameScr.arrItemStack[num2];
				break;
			case 7:
				item = GameScr.arrItemStackLock[num2];
				break;
			case 8:
				item = GameScr.arrItemGrocery[num2];
				break;
			case 9:
				item = GameScr.arrItemGroceryLock[num2];
				break;
			case 14:
				item = GameScr.arrItemStore[num2];
				break;
			case 15:
				item = GameScr.arrItemBook[num2];
				break;
			case 16:
				item = GameScr.arrItemLien[num2];
				break;
			case 17:
				item = GameScr.arrItemNhan[num2];
				break;
			case 18:
				item = GameScr.arrItemNgocBoi[num2];
				break;
			case 19:
				item = GameScr.arrItemPhu[num2];
				break;
			case 20:
				item = GameScr.arrItemNonNam[num2];
				break;
			case 21:
				item = GameScr.arrItemNonNu[num2];
				break;
			case 22:
				item = GameScr.arrItemAoNam[num2];
				break;
			case 23:
				item = GameScr.arrItemAoNu[num2];
				break;
			case 24:
				item = GameScr.arrItemGangTayNam[num2];
				break;
			case 25:
				item = GameScr.arrItemGangTayNu[num2];
				break;
			case 26:
				item = GameScr.arrItemQuanNam[num2];
				break;
			case 27:
				item = GameScr.arrItemQuanNu[num2];
				break;
			case 28:
				item = GameScr.arrItemGiayNam[num2];
				break;
			case 29:
				item = GameScr.arrItemGiayNu[num2];
				break;
			case 30:
				item = GameScr.arrItemTradeOrder[num2];
				break;
			case 32:
				item = GameScr.arrItemFashion[num2];
				break;
			case 34:
				item = GameScr.arrItemClanShop[num2];
				break;
			case 35:
				item = GameScr.arrItemElites[num2];
				break;
			case 39:
				item = Char.clan.items[GameScr.indexSelect];
				break;
			}
			item.expires = msg.reader().readLong();
			if (item.isTypeUIMe())
			{
				item.saleCoinLock = msg.reader().readInt();
			}
			else if (item.isTypeUIShop() || item.isTypeUIShopLock() || item.isTypeUIStore() || item.isTypeUIBook() || item.isTypeUIFashion() || item.isTypeUIClanShop())
			{
				item.buyCoin = msg.reader().readInt();
				item.buyCoinLock = msg.reader().readInt();
				item.buyGold = msg.reader().readInt();
			}
			if (item.isTypeBody() || item.isTypeMounts() || item.isTypeNgocKham())
			{
				item.sys = msg.reader().readByte();
				item.options = new MyVector();
				try
				{
					while (true)
					{
						item.options.addElement(new ItemOption(msg.reader().readUnsignedByte(), msg.reader().readInt()));
					}
				}
				catch (Exception)
				{
				}
			}
			else if (item.template.id == 233)
			{
				item.img = createImage(NinjaUtil.readByteArray(msg));
			}
			else if (item.template.id == 234)
			{
				item.img = createImage(NinjaUtil.readByteArray(msg));
			}
			else if (item.template.id == 235)
			{
				item.img = createImage(NinjaUtil.readByteArray(msg));
			}
			if (num == 5)
			{
				Char.getMyChar().updateKickOption();
			}
		}
		catch (Exception)
		{
			Out.println("loi tai day ham requet item info ---------------------------------------------------------");
		}
	}

	public void addMob(Message msg)
	{
		try
		{
			int num = msg.reader().readByte();
			for (sbyte b = 0; b < num; b++)
			{
				short mobId = msg.reader().readUnsignedByte();
				bool isDisable = msg.reader().readBoolean();
				bool isDontMove = msg.reader().readBoolean();
				bool isFire = msg.reader().readBoolean();
				bool isIce = msg.reader().readBoolean();
				bool isWind = msg.reader().readBoolean();
				int templateId = msg.reader().readUnsignedByte();
				int sys = msg.reader().readByte();
				int hp = msg.reader().readInt();
				int level = msg.reader().readUnsignedByte();
				int maxhp = msg.reader().readInt();
				short pointx = msg.reader().readShort();
				short pointy = msg.reader().readShort();
				sbyte status = msg.reader().readByte();
				sbyte levelBoss = msg.reader().readByte();
				bool isBos = msg.reader().readBoolean();
				Mob mob = new Mob(mobId, isDisable, isDontMove, isFire, isIce, isWind, templateId, sys, hp, level, maxhp, pointx, pointy, status, levelBoss, isBos, removeWhenDie: true);
				if (Mob.arrMobTemplate[mob.templateId].type != 0)
				{
					if (b % 3 == 0)
					{
						mob.dir = -1;
					}
					else
					{
						mob.dir = 1;
					}
					mob.x += 10 - b % 20;
				}
				GameScr.vMob.addElement(mob);
			}
		}
		catch (Exception)
		{
			mSystem.println("err addMob");
		}
	}

	public void addEffAuto(Message msg)
	{
		try
		{
			short id = msg.reader().readUnsignedByte();
			int x = msg.reader().readShort();
			int y = msg.reader().readShort();
			sbyte loopCount = msg.reader().readByte();
			short time = msg.reader().readShort();
			EffectAuto.addEffectAuto(id, x, y, loopCount, time, 1);
		}
		catch (Exception)
		{
			mSystem.println("err add effAuto");
		}
	}

	public void getDataEffAuto(Message msg)
	{
		try
		{
			short id = msg.reader().readUnsignedByte();
			short num = msg.reader().readShort();
			sbyte[] data = null;
			if (num > 0)
			{
				data = new sbyte[num];
				msg.reader().read(ref data);
			}
			EffectAuto.reciveData(id, data);
		}
		catch (Exception)
		{
			mSystem.println("err add effAuto");
		}
	}

	public void getImgEffAuto(Message msg)
	{
		try
		{
			short id = msg.reader().readUnsignedByte();
			sbyte[] data = NinjaUtil.readByteArray_Int(msg);
			EffectAuto.reciveImage(id, data);
		}
		catch (Exception)
		{
			mSystem.println("err getImgEffAuto");
		}
	}

	public void khamngoc(Message msg)
	{
		try
		{
			sbyte b = msg.reader().readByte();
			sbyte b2 = 1;
			Char.getMyChar().luong = msg.reader().readInt();
			Char.getMyChar().xu = msg.reader().readInt();
			Char.getMyChar().yen = msg.reader().readInt();
			switch (b)
			{
			case 0:
				if (GameScr.itemSplit != null)
				{
					GameScr.itemSplit = null;
				}
				if (GameScr.arrItemSplit != null)
				{
					for (int k = 0; k < GameScr.arrItemSplit.Length; k++)
					{
						GameScr.arrItemSplit[k] = null;
					}
				}
				break;
			case 1:
				if (GameScr.itemSplit != null)
				{
					GameScr.itemSplit.isLock = true;
					GameScr.itemSplit.upgrade = msg.reader().readByte();
					if (b2 == 1)
					{
						GameScr.effUpok = GameScr.efs[53];
						GameScr.indexEff = 0;
					}
				}
				if (GameScr.arrItemSplit != null)
				{
					for (int j = 0; j < GameScr.arrItemSplit.Length; j++)
					{
						GameScr.arrItemSplit[j] = null;
					}
				}
				break;
			case 2:
			case 3:
				if (GameScr.arrItemSplit != null)
				{
					for (int i = 0; i < GameScr.arrItemSplit.Length; i++)
					{
						GameScr.arrItemSplit[i] = null;
					}
				}
				break;
			}
			GameScr.gI().left = (GameScr.gI().center = null);
			GameScr.gI().updateKeyBuyItemUI();
			GameCanvas.endDlg();
		}
		catch (Exception)
		{
		}
	}

	public void addEffect(Message msg)
	{
		try
		{
			sbyte b = msg.reader().readByte();
			MainObject mainObject;
			if (b == 1)
			{
				int iD = msg.reader().readUnsignedByte();
				mainObject = Mob.get_Mob(iD);
			}
			else
			{
				int num = msg.reader().readInt();
				mainObject = ((num != Char.getMyChar().charID) ? GameScr.findCharInMap(num) : Char.getMyChar());
			}
			if (mainObject != null)
			{
				short id = msg.reader().readShort();
				int num2 = msg.reader().readInt();
				sbyte b2 = msg.reader().readByte();
				bool isHead = ((msg.reader().readByte() != 0) ? true : false);
				mainObject.addDataeff(id, num2, b2 * 1000, isHead);
			}
		}
		catch (Exception)
		{
		}
	}

	public void getImgEffect(Message msg)
	{
		try
		{
			short id = msg.reader().readUnsignedByte();
			sbyte[] array = NinjaUtil.readByteArray_Int(msg);
			GameData.setImgIcon(id, array);
			ImageIcon imageIcon = (ImageIcon)GameData.listImgIcon.get(string.Empty + id);
			if (imageIcon == null)
			{
				imageIcon = new ImageIcon();
				GameData.listImgIcon.put(id + string.Empty, imageIcon);
			}
			imageIcon.isLoad = false;
			imageIcon.img = createImage(array);
			if (GameMidlet.CLIENT_TYPE != 1)
			{
				RMS.saveRMSImage("ImgEffect " + id, array);
			}
		}
		catch (Exception)
		{
		}
	}

	public void getDataEffect(Message msg)
	{
		try
		{
			short num = msg.reader().readUnsignedByte();
			short num2 = msg.reader().readShort();
			sbyte[] data = null;
			if (num2 > 0)
			{
				data = new sbyte[num2];
				msg.reader().read(ref data);
			}
			((EffectData)GameData.listbyteData.get(string.Empty + num))?.setdata(data);
		}
		catch (Exception)
		{
		}
	}

	public void LoadBijuu(Message msg)
	{
		try
		{
			switch (msg.reader().readByte())
			{
			case 0:
			{
				Char myChar = Char.getMyChar();
				if (myChar == null)
				{
					break;
				}
				myChar.arrItemBijuu = new Item[5];
				for (int i = 0; i < myChar.arrItemBijuu.Length; i++)
				{
					short num = msg.reader().readShort();
					if (num != -1)
					{
						myChar.arrItemBijuu[i] = new Item();
						myChar.arrItemBijuu[i].typeUI = 41;
						myChar.arrItemBijuu[i].indexUI = i;
						myChar.arrItemBijuu[i].template = ItemTemplates.get(num);
						myChar.arrItemBijuu[i].upgrade = msg.reader().readByte();
						myChar.arrItemBijuu[i].expires = msg.reader().readLong();
						myChar.arrItemBijuu[i].sys = msg.reader().readByte();
						myChar.arrItemBijuu[i].isLock = true;
						int num2 = msg.reader().readByte();
						myChar.arrItemBijuu[i].options = new MyVector();
						for (int j = 0; j < num2; j++)
						{
							myChar.arrItemBijuu[i].options.addElement(new ItemOption(msg.reader().readUnsignedByte(), msg.reader().readInt()));
						}
					}
				}
				myChar.bijuuPoint = msg.reader().readShort();
				myChar.bijuupotential[0] = msg.reader().readShort();
				myChar.bijuupotential[1] = msg.reader().readShort();
				myChar.bijuupotential[2] = msg.reader().readShort();
				myChar.bijuupotential[3] = msg.reader().readShort();
				myChar.bijuuSkillPoint = msg.reader().readShort();
				myChar.vSkillBijuu.removeAllElements();
				sbyte b2 = msg.reader().readByte();
				for (byte b3 = 0; b3 < b2; b3++)
				{
					short skillId = msg.reader().readShort();
					Skill o = Skills.get(skillId);
					myChar.vSkillBijuu.addElement(o);
				}
				break;
			}
			case 1:
			{
				sbyte b4 = msg.reader().readByte();
				if (b4 != 0 && b4 == 1)
				{
				}
				break;
			}
			case 2:
			{
				sbyte b = msg.reader().readByte();
				if (b != 0 && b == 1)
				{
				}
				break;
			}
			}
		}
		catch (Exception)
		{
		}
	}

	private void readDataMobOld(Message msg, int mobTemplateId)
	{
		try
		{
			Mob.arrMobTemplate[mobTemplateId].imginfo = new ImageInfo[msg.reader().readByte()];
			for (int i = 0; i < Mob.arrMobTemplate[mobTemplateId].imginfo.Length; i++)
			{
				Mob.arrMobTemplate[mobTemplateId].imginfo[i] = new ImageInfo();
				Mob.arrMobTemplate[mobTemplateId].imginfo[i].ID = msg.reader().readByte();
				Mob.arrMobTemplate[mobTemplateId].imginfo[i].x0 = msg.reader().readUnsignedByte();
				Mob.arrMobTemplate[mobTemplateId].imginfo[i].y0 = msg.reader().readUnsignedByte();
				Mob.arrMobTemplate[mobTemplateId].imginfo[i].w = msg.reader().readUnsignedByte();
				Mob.arrMobTemplate[mobTemplateId].imginfo[i].h = msg.reader().readUnsignedByte();
			}
			Mob.arrMobTemplate[mobTemplateId].frameBoss = new Frame[msg.reader().readShort()];
			for (int j = 0; j < Mob.arrMobTemplate[mobTemplateId].frameBoss.Length; j++)
			{
				Mob.arrMobTemplate[mobTemplateId].frameBoss[j] = new Frame();
				sbyte b = msg.reader().readByte();
				Mob.arrMobTemplate[mobTemplateId].frameBoss[j].dx = new short[b];
				Mob.arrMobTemplate[mobTemplateId].frameBoss[j].dy = new short[b];
				Mob.arrMobTemplate[mobTemplateId].frameBoss[j].idImg = new sbyte[b];
				for (int k = 0; k < b; k++)
				{
					Mob.arrMobTemplate[mobTemplateId].frameBoss[j].dx[k] = msg.reader().readShort();
					Mob.arrMobTemplate[mobTemplateId].frameBoss[j].dy[k] = msg.reader().readShort();
					Mob.arrMobTemplate[mobTemplateId].frameBoss[j].idImg[k] = msg.reader().readByte();
				}
			}
			short num = msg.reader().readShort();
			for (int l = 0; l < num; l++)
			{
				msg.reader().readShort();
			}
		}
		catch (Exception)
		{
		}
	}

	private void readDataMobNew(Message msg, int mobTemplateId)
	{
		try
		{
			bool flag = true;
			Mob.arrMobTemplate[mobTemplateId].imginfo = new ImageInfo[msg.reader().readByte()];
			for (int i = 0; i < Mob.arrMobTemplate[mobTemplateId].imginfo.Length; i++)
			{
				Mob.arrMobTemplate[mobTemplateId].imginfo[i] = new ImageInfo();
				Mob.arrMobTemplate[mobTemplateId].imginfo[i].ID = msg.reader().readByte();
				Mob.arrMobTemplate[mobTemplateId].imginfo[i].x0 = msg.reader().readUnsignedByte();
				Mob.arrMobTemplate[mobTemplateId].imginfo[i].y0 = msg.reader().readUnsignedByte();
				Mob.arrMobTemplate[mobTemplateId].imginfo[i].w = msg.reader().readUnsignedByte();
				Mob.arrMobTemplate[mobTemplateId].imginfo[i].h = msg.reader().readUnsignedByte();
			}
			Mob.arrMobTemplate[mobTemplateId].frameBoss = new Frame[msg.reader().readShort()];
			for (int j = 0; j < Mob.arrMobTemplate[mobTemplateId].frameBoss.Length; j++)
			{
				Mob.arrMobTemplate[mobTemplateId].frameBoss[j] = new Frame();
				sbyte b = msg.reader().readByte();
				Mob.arrMobTemplate[mobTemplateId].frameBoss[j].dx = new short[b];
				Mob.arrMobTemplate[mobTemplateId].frameBoss[j].dy = new short[b];
				Mob.arrMobTemplate[mobTemplateId].frameBoss[j].idImg = new sbyte[b];
				Mob.arrMobTemplate[mobTemplateId].frameBoss[j].flip = new sbyte[b];
				Mob.arrMobTemplate[mobTemplateId].frameBoss[j].onTop = new sbyte[b];
				for (int k = 0; k < b; k++)
				{
					Mob.arrMobTemplate[mobTemplateId].frameBoss[j].dx[k] = msg.reader().readShort();
					Mob.arrMobTemplate[mobTemplateId].frameBoss[j].dy[k] = msg.reader().readShort();
					Mob.arrMobTemplate[mobTemplateId].frameBoss[j].idImg[k] = msg.reader().readByte();
					if (flag)
					{
						Mob.arrMobTemplate[mobTemplateId].frameBoss[j].flip[k] = msg.reader().readByte();
						Mob.arrMobTemplate[mobTemplateId].frameBoss[j].onTop[k] = msg.reader().readByte();
					}
				}
			}
			short num = 0;
			num = ((!flag) ? msg.reader().readShort() : msg.reader().readUnsignedByte());
			Mob.arrMobTemplate[mobTemplateId].sequence = new sbyte[num];
			for (int l = 0; l < num; l++)
			{
				Mob.arrMobTemplate[mobTemplateId].sequence[l] = (sbyte)msg.reader().readShort();
			}
			if (flag)
			{
				msg.reader().readByte();
				for (int m = 0; m < 4; m++)
				{
					if (m != 2)
					{
						num = msg.reader().readByte();
						Mob.arrMobTemplate[mobTemplateId].frameChar[m] = new sbyte[num];
						for (int n = 0; n < num; n++)
						{
							Mob.arrMobTemplate[mobTemplateId].frameChar[m][n] = msg.reader().readByte();
						}
					}
				}
			}
			try
			{
				Mob.arrMobTemplate[mobTemplateId].indexSplash[0] = (sbyte)(Mob.arrMobTemplate[mobTemplateId].frameChar[0].Length - 7);
				Mob.arrMobTemplate[mobTemplateId].indexSplash[1] = (sbyte)(Mob.arrMobTemplate[mobTemplateId].frameChar[1].Length - 7);
				Mob.arrMobTemplate[mobTemplateId].indexSplash[2] = (sbyte)(Mob.arrMobTemplate[mobTemplateId].frameChar[3].Length - 7);
				Mob.arrMobTemplate[mobTemplateId].indexSplash[3] = (sbyte)(Mob.arrMobTemplate[mobTemplateId].frameChar[3].Length - 7);
			}
			catch (Exception)
			{
			}
			for (int num2 = 0; num2 < 3; num2++)
			{
				Mob.arrMobTemplate[mobTemplateId].indexSplash[num2] = msg.reader().readByte();
			}
			Mob.arrMobTemplate[mobTemplateId].indexSplash[3] = Mob.arrMobTemplate[mobTemplateId].indexSplash[2];
			Mob.arrMobTemplate[mobTemplateId].imginfo = new ImageInfo[msg.reader().readByte()];
		}
		catch (Exception)
		{
		}
	}

	public void readDataMobNew(sbyte[] data, int mobTemplateId)
	{
		DataInputStream dataInputStream = null;
		try
		{
			dataInputStream = new DataInputStream(data);
			bool flag = true;
			Mob.arrMobTemplate[mobTemplateId].imginfo = new ImageInfo[dataInputStream.readByte()];
			for (int i = 0; i < Mob.arrMobTemplate[mobTemplateId].imginfo.Length; i++)
			{
				Mob.arrMobTemplate[mobTemplateId].imginfo[i] = new ImageInfo();
				Mob.arrMobTemplate[mobTemplateId].imginfo[i].ID = dataInputStream.readByte();
				Mob.arrMobTemplate[mobTemplateId].imginfo[i].x0 = dataInputStream.readUnsignedByte();
				Mob.arrMobTemplate[mobTemplateId].imginfo[i].y0 = dataInputStream.readUnsignedByte();
				Mob.arrMobTemplate[mobTemplateId].imginfo[i].w = dataInputStream.readUnsignedByte();
				Mob.arrMobTemplate[mobTemplateId].imginfo[i].h = dataInputStream.readUnsignedByte();
			}
			Mob.arrMobTemplate[mobTemplateId].frameBoss = new Frame[dataInputStream.readShort()];
			for (int j = 0; j < Mob.arrMobTemplate[mobTemplateId].frameBoss.Length; j++)
			{
				Mob.arrMobTemplate[mobTemplateId].frameBoss[j] = new Frame();
				sbyte b = dataInputStream.readByte();
				Mob.arrMobTemplate[mobTemplateId].frameBoss[j].dx = new short[b];
				Mob.arrMobTemplate[mobTemplateId].frameBoss[j].dy = new short[b];
				Mob.arrMobTemplate[mobTemplateId].frameBoss[j].idImg = new sbyte[b];
				Mob.arrMobTemplate[mobTemplateId].frameBoss[j].flip = new sbyte[b];
				Mob.arrMobTemplate[mobTemplateId].frameBoss[j].onTop = new sbyte[b];
				for (int k = 0; k < b; k++)
				{
					Mob.arrMobTemplate[mobTemplateId].frameBoss[j].dx[k] = dataInputStream.readShort();
					Mob.arrMobTemplate[mobTemplateId].frameBoss[j].dy[k] = dataInputStream.readShort();
					Mob.arrMobTemplate[mobTemplateId].frameBoss[j].idImg[k] = dataInputStream.readByte();
					if (flag)
					{
						Mob.arrMobTemplate[mobTemplateId].frameBoss[j].flip[k] = dataInputStream.readByte();
						Mob.arrMobTemplate[mobTemplateId].frameBoss[j].onTop[k] = dataInputStream.readByte();
					}
				}
			}
			short num = 0;
			num = ((!flag) ? dataInputStream.readShort() : ((short)dataInputStream.readUnsignedByte()));
			Mob.arrMobTemplate[mobTemplateId].sequence = new sbyte[num];
			for (int l = 0; l < num; l++)
			{
				Mob.arrMobTemplate[mobTemplateId].sequence[l] = (sbyte)dataInputStream.readShort();
			}
			if (flag)
			{
				dataInputStream.readByte();
				for (int m = 0; m < 4; m++)
				{
					if (m != 2)
					{
						num = dataInputStream.readByte();
						Mob.arrMobTemplate[mobTemplateId].frameChar[m] = new sbyte[num];
						for (int n = 0; n < num; n++)
						{
							Mob.arrMobTemplate[mobTemplateId].frameChar[m][n] = dataInputStream.readByte();
						}
					}
				}
			}
			try
			{
				Mob.arrMobTemplate[mobTemplateId].indexSplash[0] = (sbyte)(Mob.arrMobTemplate[mobTemplateId].frameChar[0].Length - 7);
				Mob.arrMobTemplate[mobTemplateId].indexSplash[1] = (sbyte)(Mob.arrMobTemplate[mobTemplateId].frameChar[1].Length - 7);
				Mob.arrMobTemplate[mobTemplateId].indexSplash[2] = (sbyte)(Mob.arrMobTemplate[mobTemplateId].frameChar[3].Length - 7);
				Mob.arrMobTemplate[mobTemplateId].indexSplash[3] = (sbyte)(Mob.arrMobTemplate[mobTemplateId].frameChar[3].Length - 7);
			}
			catch (Exception)
			{
			}
			for (int num2 = 0; num2 < 3; num2++)
			{
				Mob.arrMobTemplate[mobTemplateId].indexSplash[num2] = dataInputStream.readByte();
			}
			Mob.arrMobTemplate[mobTemplateId].indexSplash[3] = Mob.arrMobTemplate[mobTemplateId].indexSplash[2];
			Mob.arrMobTemplate[mobTemplateId].imginfo = new ImageInfo[dataInputStream.readByte()];
		}
		catch (Exception)
		{
		}
	}
}
