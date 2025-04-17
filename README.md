🎮 TÊN GAME: LIÊN FIRE  
Thể loại: MOBA 3D kết hợp TPS (Third-Person Shooter) – Multiplayer

======================================
I. TỔNG QUAN GAME
======================================
- Góc nhìn: TPS – camera từ sau lưng nhân vật
- Engine: Unity 3D
- Multiplayer: 3 vs 3 dùng Photon PUN 2
- Sảnh chờ (lobby): Có thể bắt đầu nếu mỗi đội có ít nhất 1 người
- Bản đồ: 3 lane (Top – Mid – Bot) + rừng, trụ, nhà chính
- Tướng (Hero): Nhiều loại nhân vật với bộ kỹ năng riêng
- Chế độ chơi: PvP 3v3, hoặc đấu AI nếu thiếu người
- Minimap: Có hiển thị đồng minh, kẻ địch, trụ, quái, nhà chính
- Quái rừng: Spawn tại vị trí cố định, rớt buff tạm thời
- Item nhặt được: Tăng chỉ số trong khoảng thời gian

======================================
II. HỆ THỐNG NHÂN VẬT (HERO SYSTEM)
======================================
Mỗi nhân vật có:
- 3 kỹ năng chủ động (Q, E, R)
- 1 kỹ năng đánh thường (Chuột trái)
- 1 kỹ năng bổ trợ (F)

Chỉ số cơ bản:
1. Máu (HP)
2. Sát thương đánh thường
3. Tốc độ đánh
4. Tầm đánh
5. Tốc độ di chuyển

Mỗi tướng có vai trò riêng (cận chiến, đánh xa, hỗ trợ)

Kỹ năng có cooldown, hiệu ứng hình ảnh và âm thanh

Dữ liệu tướng được quản lý bằng ScriptableObject để dễ cấu hình

======================================
III. MAP 3-LANE + QUÁI RỪNG
======================================
Bản đồ gồm 3 lane: Top, Mid, Bot

Mỗi lane có:
- 2 trụ phòng thủ
- 1 nhà chính

Khu vực rừng có 5 điểm quái cố định:
- 2 điểm ở Top
- 2 điểm ở Bot
- 1 điểm giữa Mid (boss nhỏ)

Quái:
- Đứng yên hoặc di chuyển giới hạn
- Tấn công nếu có player hoặc minion đến gần
- Khi chết rớt item tăng chỉ số tạm thời
- Tái sinh sau thời gian nhất định

======================================
IV. HỆ THỐNG AI
======================================
Lính lane:
- Tự di chuyển theo lane
- Tấn công kẻ địch gần nhất: lính → trụ → nhà chính

Enemy AI:
- Có hành vi dựa trên FSM (Patrol, Detect, Attack, Retreat)
- Có 3 loại: 
  + MeleeBot – cận chiến, nhiều máu
  + RangedBot – đánh xa, máu ít
  + EliteBot – mạnh hơn, có kỹ năng

======================================
V. ITEM BOOST TẠM THỜI
======================================
- Xuất hiện từ quái rừng hoặc đặt trên map
- Tăng chỉ số cho người nhặt trong thời gian giới hạn
- Các loại:
  + Tăng sát thương
  + Tăng tốc đánh
  + Tăng tốc chạy
  + Tăng tầm đánh
  + Hồi máu

Hết thời gian thì hiệu ứng buff mất đi

======================================
VI. MULTIPLAYER (PHOTON PUN 2)
======================================
- Sử dụng Photon PUN 2 để kết nối nhiều người chơi
- Sảnh chờ có thể bắt đầu khi đủ 1 người mỗi bên
- Mỗi phòng tối đa 6 người (3 vs 3)
- Người giữ quyền host (MasterClient) xử lý AI, spawn lính, quái rừng
- Sử dụng PhotonView, PhotonTransformView, RPC để đồng bộ:
  + Di chuyển
  + Kỹ năng
  + Giao tiếp
  + Tấn công và hiệu ứng

======================================
VII. UI + MINIMAP (SỬ DỤNG CANVAS UI)
======================================
- Dùng Canvas UI để tạo giao diện game

HUD bao gồm:
- Thanh máu hiển thị máu nhân vật
- Icon kỹ năng Q E R và bổ trợ F với thanh cooldown
- Khu vực hiển thị buff tạm thời đang có
- Số kill, số chết, hoặc thông báo từ hệ thống

Minimap:
- Dùng camera trên cao + RenderTexture + RawImage trong UI
- Hiển thị icon cho:
  + Người chơi đồng minh
  + Kẻ địch (nếu phát hiện)
  + Trụ, nhà chính
  + Quái rừng còn sống

======================================
VIII. LỘ TRÌNH PHÁT TRIỂN (6 TUẦN)
======================================
Tuần 1:
- Tạo project Unity 3D, tích hợp Photon
- Làm sảnh chờ, tạo/join phòng

Tuần 2:
- Tạo nhân vật góc nhìn TPS
- Di chuyển, camera Cinemachine, tấn công thường

Tuần 3:
- Thiết kế bản đồ 3 lane
- Tạo hệ thống trụ, nhà chính, spawn minion lane

Tuần 4:
- Làm quái rừng
- Thiết lập hệ thống item tăng chỉ số tạm thời

Tuần 5:
- Tích hợp Photon cho đồng bộ nhân vật, kỹ năng, AI, item
- Cân bằng gameplay

Tuần 6:
- Làm UI bằng Canvas: máu, kỹ năng, minimap, bảng thông báo
- Polish, test toàn bộ, viết báo cáo

======================================
IX. TÀI NGUYÊN VÀ CÔNG CỤ
======================================
- Unity 3D: Engine phát triển game
- Photon PUN 2: Hệ thống multiplayer
- Cinemachine: Camera góc nhìn TPS
- NavMesh: Di chuyển AI
- Mixamo: Nhân vật và animation miễn phí
- ScriptableObject: Quản lý dữ liệu tướng và kỹ năng
- Canvas UI: Tạo giao diện (HUD, kỹ năng, minimap)

