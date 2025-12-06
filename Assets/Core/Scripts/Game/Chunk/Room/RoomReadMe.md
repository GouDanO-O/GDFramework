# 房间网格编辑器 - 集成指南

## 最新修复 (版本更新)

### 修复1：切换模式问题
**问题**：按2进入TileEdit模式后，需要重复按键才能切换地块  
**解决**：重写 `RoomGridEditorInput.cs`，使用单一的 Unity 原生 Input 系统，添加 `_hasFiredClick` 防止重复触发

### 修复2：拖拽绘制无效
**问题**：拖拽时日志显示位置变化，但地块没有被修改  
**解决**：修复 `RoomGridEditor.cs` 中的拖拽处理逻辑，确保在拖拽开始时正确设置 `State.IsOperating`

### 修复3：合批失效（7584 Batches）
**问题**：7584个Batches说明没有正确合并Mesh  
**解决**：重写 `TileChunk.cs`，使用单一 Mesh + 多 SubMesh 方案

**新增功能**：
- 在 TileRenderer Inspector 中点击 **"使用共享材质（最大合批）"** 按钮
- 所有地块类型使用同一材质（通过顶点颜色区分）
- 可实现最少的 DrawCall（约10-20个）

---

## 文件结构

将以下文件复制到你的项目：

```
Assets/Core/Scripts/Game/Chunk/Room/
├── Grid/
│   ├── Enums/
│   │   ├── TileType.cs              // 地块类型枚举
│   │   └── PlacementMode.cs         // 编辑模式枚举
│   ├── Data/
│   │   ├── TileData.cs              // 地块数据
│   │   ├── PlacedObjectData.cs      // 放置物品数据
│   │   └── RoomGridConfig.cs        // 网格配置
│   ├── RoomGrid.cs                  // 网格核心逻辑
│   ├── Renderer/
│   │   ├── TileRenderer.cs          // 地块渲染器（合并Mesh）
│   │   ├── TileChunk.cs             // 渲染块组件
│   │   └── PreviewRenderer.cs       // 预览渲染器
│   └── RoomGridEditor/
│       ├── RoomGridEditorState.cs   // 编辑器状态机
│       ├── RoomGridEditorInput.cs   // 输入处理
│       ├── RoomGridEditorCamera.cs  // 相机控制
│       ├── RoomGridEditor.cs        // 编辑器主控制器
│       └── UI/
│           └── RoomEditorUIPanel.cs // UI面板
└── Test/
    ├── RoomGridEditorTest.cs        // 测试脚本
    ├── InputDebugger.cs             // 输入调试器
    └── InputSystemInitializer.cs    // 输入系统初始化
```

## 测试场景搭建

### 步骤 1：创建测试场景

1. 创建新场景：`File -> New Scene`
2. 保存为：`RoomEditorTest.unity`

### 步骤 2：创建编辑器对象

1. 创建空物体：`GameObject -> Create Empty`
2. 命名为：`RoomGridEditor`
3. 添加以下组件：
    - `RoomGridEditor`
    - `RoomGridEditorTest`
    - `TileRenderer`
    - `PreviewRenderer`

### 步骤 3：设置地块渲染器

在 `TileRenderer` 组件上：
1. 点击 **"自动生成默认材质"** 按钮
2. 或手动配置每种地块类型的材质

### 步骤 4：创建相机

1. 创建空物体作为相机Pivot：命名为 `CameraPivot`
2. 将 Main Camera 移动到 `CameraPivot` 下作为子物体
3. 给 `CameraPivot` 添加 `RoomGridEditorCamera` 组件
4. 设置相机初始位置：
    - CameraPivot Position: (25, 0, 25) —— 网格中心
    - Main Camera Local Position: (0, 0, -20)

### 步骤 5：确保输入系统初始化

在你的游戏启动流程中，确保调用了：
```csharp
this.GetSystem<NewInputManager>().InitActionAsset();
```

或者添加 `InputSystemInitializer` 组件到场景中。

## 操作说明

### 快捷键

| 按键 | 功能 |
|------|------|
| `1` | 查看模式 |
| `2` | 地块编辑模式 |
| `3` | 物品放置模式 |
| `4` | 选择模式 |
| `5` | 删除模式 |
| `Q/E` | 切换地块类型 |
| `Tab` | 切换工具 |
| `R` | 旋转物品 |
| `Delete` | 删除选中 |
| `Esc` | 取消/返回 |
| `WASD` | 移动相机 |
| 中键拖拽 | 平移相机 |
| 右键拖拽 | 旋转相机 |
| 滚轮 | 缩放相机 |
| `[` / `]` | 调整画笔大小 |

### 编辑模式说明

- **View（查看）**：只能查看，不能编辑
- **TileEdit（地块编辑）**：绘制/擦除地块
    - Brush：画笔工具，点击或拖拽绘制
    - Fill：洪水填充，填充连续区域
    - Rectangle：矩形工具，拖拽绘制矩形
    - Eraser：橡皮擦，擦除地块
- **ObjectPlace（物品放置）**：放置物品到地块上
- **ObjectSelect（选择）**：选择已放置的物品
- **Delete（删除）**：删除地块或物品

## 渲染器说明

### TileRenderer（地块渲染器）

使用合并 Mesh 方案实现高性能渲染：
- 将地块按区域分块（默认16x16）
- 每个块内按材质合并网格
- 支持延迟批量更新，避免频繁重建

配置项：
- **地块厚度**：地块的高度
- **地块间隙**：地块之间的缝隙
- **高度单位**：每个高度等级的实际高度
- **分块大小**：每个渲染块的尺寸
- **延迟更新时间**：批量更新的延迟

### PreviewRenderer（预览渲染器）

显示放置前的预览效果：
- 绿色半透明：可以放置
- 红色半透明：不能放置

## 常见问题

### Q: 点击没有反应？
A: 检查：
1. 是否调用了 `NewInputManager.InitActionAsset()`
2. 是否切换到了编辑模式（按 `2`）
3. 查看 Console 是否有日志输出

### Q: 看不到地块渲染？
A: 检查：
1. `TileRenderer` 是否添加到场景
2. 是否点击了"自动生成默认材质"
3. 相机是否对准网格区域

### Q: 预览不显示？
A: 检查：
1. `PreviewRenderer` 是否添加到场景
2. 是否在 TileEdit/ObjectPlace/Delete 模式下

## 下一步

1. ✅ 核心数据结构
2. ✅ 运行时编辑器
3. ✅ 地块渲染器（合并Mesh）
4. ✅ 预览渲染器
5. ✅ 物品渲染器
6. ✅ 物品定义系统
7. ✅ 序列化/存档系统
8. ⬜ 与现有 Room 系统整合
9. ⬜ UI 面板完善（视觉设计）

## 新增功能说明

### 物品系统

**物品定义管理器** (`ObjectDefinitionManager`)：
- 支持从JSON加载物品定义
- 按类别分类管理
- 内置测试数据（桌子、椅子、沙发、床、地毯、盆栽等）

**物品合批渲染器** (`ObjectBatchRenderer`)：
- 按类别分组合批渲染，大幅减少 DrawCall
- 同类别物品合并到一个 Mesh
- 支持射线检测和物品选择
- 自动更新脏批次

**性能对比**：
| 方案 | 100个物品的DrawCall |
|------|---------------------|
| 旧方案（每物品一个GameObject） | ~600+ |
| 新方案（按类别合批） | ~8（每类别1个） |

### 存档系统

**存档管理器** (`RoomGridSaveSystem`)：
- 保存/加载房间数据到文件
- 支持缩略图
- 支持元数据
- 快速保存(F5)/快速加载(F9)

### 快捷键更新

| 按键 | 地块编辑模式 | 物品放置模式 |
|------|-------------|-------------|
| `Q/E` | 切换地块类型 | 切换物品 |
| `Tab` | 切换工具 | 切换物品类别 |
| `F5` | 快速保存 | 快速保存 |
| `F9` | 快速加载 | 快速加载 |