---
name: nso-client-development
description: Hướng dẫn kiến trúc, các component, quy ước mạng, hệ thống game và cách phát triển/modding cho Unity Ninja School Online Client (NSO Client).
---

# NSO Client - Architecture & Component Skill Guide

Tài liệu hướng dẫn toàn diện về kiến trúc mã nguồn, các hệ thống lõi (Core Systems), Network Protocol, Entity, UI, và hệ thống Auto/Mod của dự án **Client NSO (Unity C#)**.

---

## 1. Tổng quan Kiến trúc (High-Level Architecture)

Dự án là Client game Ninja School Online được chuyển đổi (port) từ nền tảng J2ME sang Unity C#, giữ nguyên mô hình game loop tùy biến (Custom Game Engine Canvas), vẽ đồ họa 2D dạng pixel-matrix trên Unity `mGraphics`, và quản lý mạng theo socket TCP nhị phân (Binary Protocol).

```
[Unity Engine (Main.cs / MonoBehaviour)]
         │
         ├──> [GameCanvas (Core Loop: update(), paint(), input)]
         │         │
         │         ├──> [mScreen (SplashScr -> LoginScr -> SelectCharScr -> GameScr)]
         │         ├──> [UI: Menu, Dialog, MsgDlg, InputDlg, TField, Scroll]
         │         └──> [Rendering: mGraphics, mFont, SmallImage, FrameImage, Effect]
         │
         ├──> [World & Entities]
         │         ├──> [TileMap: Matrix collision, background, tiles, waypoints]
         │         ├──> [Char (myChar + Other Players)]
         │         ├──> [Mob & MobTemplate (Monsters & Bosses)]
         │         ├──> [Npc & NpcTemplate (Interactive NPCs)]
         │         └──> [Item & ItemMap (Inventory, Equipment, Drops)]
         │
         ├──> [Network Subsystem]
         │         ├──> [Session_ME (TCP Socket, threads Sender/Receiver)]
         │         ├──> [Service (Outgoing Packet Factory)]
         │         ├──> [Controller (Incoming Packet Dispatcher & Handler)]
         │         └──> [Cmd & Message (Binary DataInputStream / DataOutputStream)]
         │
         └──> [Mod & Automation Subsystem]
                   ├──> [Code.cs (Chat Commands / Shortcuts / Utilities)]
                   ├──> [MenuAuto.cs (In-game Hack/Auto UI)]
                   ├──> [Auto.cs, AutoTask.cs, TanSat.cs, Buff.cs]
                   └──> [CLock.cs (Anti-lock / Timer utils)]
```

---

## 2. Phân loại các Component chính (Core Components)

### 2.1. Nhóm Khởi tạo & Game Engine Canvas
| File | Vai trò & Trách nhiệm |
|---|---|
| `Main.cs` | Kế thừa `MonoBehaviour`. Thiết lập FPS (30/60), độ phân giải, Orientation, gọi `ScaleGUI.initScaleGUI()`, khởi tạo `GameCanvas`, nạp mảng `TileMap.loadTileMapArr()` và chuyển sang `SplashScr`. |
| `GameCanvas.cs` | Quản lý toàn bộ vòng đời `paint()` và `update()`, phân phối sự kiện chuột/chạm/bàn phím tới màn hình hiện tại (`currentScreen`). Quản lý popup, dialog, menu toàn cục. |
| `MotherCanvas.cs` | Cầu nối giữa Unity OnGUI/Update với `GameCanvas`. |
| `ScaleGUI.cs` | Chuẩn hóa tỉ lệ hiển thị trên mọi độ phân giải màn hình thiết bị (PC / Mobile). |
| `RMS.cs` | Record Management System - Mô phỏng hệ thống lưu trữ dữ liệu cục bộ (PlayerPrefs / Local Files). |

### 2.2. Nhóm Mạng & Giao thức (Network & Protocol)
| File | Vai trò & Trách nhiệm |
|---|---|
| `Session_ME.cs` | Kết nối Socket TCP đa luồng (1 Thread gửi, 1 Thread nhận), xử lý bắt tay mã hóa khóa phiên (key exchange/handshake). |
| `Message.cs` | Đại diện cho một frame dữ liệu binary gồm `command` (byte) và payload `DataInputStream` / `DataOutputStream`. |
| `Cmd.cs` | Định nghĩa toàn bộ hằng số ID của các gói tin gửi/nhận giữa Client và Server. |
| `Service.cs` | Chứa các hàm đóng gói dữ liệu và gửi Message lên Server (`login`, `move`, `attack`, `useItem`, `openMenu`,...). |
| `Controller.cs` | Implement `IMessageHandler`, nhận các Message từ server trả về, giải mã payload và cập nhật state game (máu, tọa độ, đồ đạc, quái, nhân vật khác). |

### 2.3. Nhóm Đối tượng & Thế giới Game (Entities & World)
| File | Vai trò & Trách nhiệm |
|---|---|
| `TileMap.cs` | Bản đồ tile 24x24 pixel. Xử lý va chạm vật lý (đất đứng, tường, nước, bụi rậm, hố gai), tải map, chuyển khu, render layer cảnh nền (Background) và mây trời/mưa tuyết. |
| `Char.cs` | Đối tượng nhân vật. Xử lý tọa độ `(cx, cy)`, animation (đứng, đi, nhảy, chém, gục), trang bị (vũ khí, nón, áo, quần, giày, thú cưỡi), máu/mana, buff hiệu ứng. `Char.getMyChar()` là nhân vật chính của người chơi. |
| `Mob.cs` | Quái vật và Boss. Quản lý máu, trạng thái đơ, bị đánh, tấn công người chơi, nhặt vị trí spawn và animation. |
| `Npc.cs` | NPC đối thoại và trao đổi nhiệm vụ trong map. |
| `Item.cs` | Vật phẩm trong rương, hành trang và trang bị trên người (bao gồm option chỉ số, cường hóa, hạn sử dụng). |
| `ItemMap.cs` | Vật phẩm rơi trên mặt đất khi đánh quái hoặc vứt ra. |
| `Skill.cs` | Kỹ năng môn phái (kiếm, phi tiêu, kunai, cung, đao, quạt), cấp độ và cooldown. |

### 2.4. Nhóm Giao diện (Screens & UI)
| File | Vai trò & Trách nhiệm |
|---|---|
| `mScreen.cs` | Lớp trừu tượng cho tất cả màn hình trong game. |
| `SplashScr.cs` | Màn hình chờ mở đầu game. |
| `LoginScr.cs` | Màn hình đăng nhập tài khoản/mật khẩu, ghi nhớ nick. |
| `SelectServerScr.cs` | Màn hình chọn server (World, Local, Server test). |
| `SelectCharScr.cs` / `CreateCharScr.cs` | Chọn và tạo nhân vật mới (chọn giới tính, tóc, làng). |
| `GameScr.cs` | Màn hình chơi game chính: vẽ nhân vật, map, quái, thanh HUD HP/MP, tab chat, joystick điều khiển, shortcut kỹ năng, mở rương đồ. |
| `Menu.cs`, `Dialog.cs`, `MsgDlg.cs`, `InputDlg.cs` | Hệ thống menu lựa chọn, popup thông báo, popup nhập văn bản/số lượng. |
| `TField.cs` | Text field nhập liệu. |

### 2.5. Nhóm Đồ họa & Hiệu ứng (Graphics & Rendering)
| File | Vai trò & Trách nhiệm |
|---|---|
| `mGraphics.cs` | Wrapper đồ họa render hình ảnh, hỗ trợ lật hình (`TRANS_MIRROR`), xoay góc, cắt vùng (`setClip`), vẽ chuỗi ký tự. |
| `mFont.cs` | Hệ thống vẽ chữ bitmap nhiều màu (vàng, đỏ, trắng, xanh lá, xanh dương,...). |
| `SmallImage.cs` | Bộ nhớ đệm quản lý hình ảnh nhỏ và icon từ server/local. |
| `DataSkillEff.cs`, `Effect.cs`, `ServerEffect.cs` | Hiệu ứng kỹ năng, hào quang, nổ, sét giật, cánh, trang bị phát sáng. |

### 2.6. Nhóm Tiện ích Auto & Mod (Modding / Automation)
| File | Vai trò & Trách nhiệm |
|---|---|
| `Code.cs` | Trung tâm điều khiển lệnh chat tắt (vd: `ts`, `ak`, `hs`, `nm`, `k`, `c`), phím tắt, tăng tốc game (hack giày/speed), auto chat. |
| `MenuAuto.cs` | Menu cài đặt tính năng auto trong game (Tàn sát, Auto nhiệm vụ, Buff, Gom quái, Auto hút đồ,...). |
| `Auto.cs`, `AutoTask.cs` | Thuật toán auto làm nhiệm vụ, tự động tìm NPC, đánh quái mục tiêu, chuyển map. |
| `TanSat.cs` | Logic tàn sát quái tự động (chọn quái gần nhất, di chuyển, dùng skill, tránh quái né đòn). |
| `Buff.cs` | Logic tự động sử dụng chiêu thức hỗ trợ/hồi phục (cho bản thân hoặc thành viên tổ đội). |

---

## 3. Quy ước và Luồng hoạt động chính (Standard Workflows)

### 3.1. Luồng Gửi & Nhận Gói tin (Packet Flow)
1. **Gửi dữ liệu**:
   `Service.gI().someAction(...)` -> Tạo `Message(cmd)` -> Ghi dữ liệu vào `DataOutputStream` -> `Session_ME.gI().sendMessage(msg)`.
2. **Nhận dữ liệu**:
   `Session_ME` (Receiver thread) -> Đọc byte `cmd` và payload -> Tạo `Message` -> Đưa vào `Controller.gI().onMessage(msg)` -> Gọi hàm xử lý tương ứng -> Cập nhật Model / View (`GameScr`, `Char`, `Mob`).

### 3.2. Luồng Game Loop
1. Unity `Update()` -> Gọi `GameCanvas.update()` -> Cập nhật `currentScreen.update()` -> Cập nhật vị trí nhân vật, quái, hạt hiệu ứng.
2. Unity `OnGUI()` -> Gọi `GameCanvas.paint(g)` -> `currentScreen.paint(g)` -> Render TileMap layer dưới -> Render Quái / Item / Người chơi -> Render TileMap layer trên -> Render Effect -> Render UI/HUD/Dialog.

---

## 4. Hướng dẫn thêm tính năng mới hoặc Mod Client

1. **Thêm Lệnh Chat mới**:
   - Mở `Assets/Scripts/Code.cs`.
   - Tìm hàm `ChatMod(string text)`.
   - Thêm nhánh lệnh kiểm tra `text.Equals("ten_lenh")` hoặc tiền tố `text.StartsWith("...")`.
2. **Thêm Gói tin Mới (Network Packet)**:
   - Khai báo hằng số lệnh trong `Cmd.cs`.
   - Tạo phương thức gửi trong `Service.cs`.
   - Thêm case xử lý trong `Controller.cs` (hàm `onMessage` hoặc phương thức phụ).
3. **Thêm UI / Menu Tùy Chỉnh**:
   - Dùng `MenuAuto.cs` hoặc gọi trực tiếp `GameCanvas.menu.startAt(myVector, pos)` để mở menu popup với danh sách các `Command(title, actionListener, idAction, obj)`.
