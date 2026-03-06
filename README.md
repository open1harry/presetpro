# Preset Pro

`Preset Pro` 是一个 Unity 编辑器效率插件，用于快速保存、整理、备份 Prefab 预设，并一键生成 `GameObject` 菜单入口。

它特别适合美术和特效工作流：你可以把常用对象快速沉淀为可复用的预设库，按分类整理，并通过 Unity 原生菜单快速实例化。

## 核心价值

- 快速保存和备份场景中的对象为 Prefab 预设
- 基于文件夹分类，天然支持批量整理、导入导出
- 支持一键同步到 `GameObject/<自定义名称>/...` 菜单
- 适合搭建个人或团队的特效模板库 / 预设资产库

## 功能特性

- 文件夹即分类，Prefab 即选项
- 顶级工具菜单位于 `VFXSkill -> Preset Pro`（英文）或 `VFXSkill -> 预设 Pro`（中文）
- 顶级菜单会根据 `uiLanguage` 切换当前显示语言
- `Shift + V` 快捷保存（Quick Add）
- 自动生成 Unity `MenuItem` 菜单代码
- 首次打开自动显示工具介绍
- 默认中文界面，可切换英文
- 支持自定义 `GameObject` 菜单根节点名称
- 支持在设置中调整分类顺序，生成菜单时按排序输出
- 预设文件夹路径在设置中只读显示，通过选择器修改

## 兼容版本

- Unity `2022.3.x`（开发与验证版本）

## 安装教程（Unity）

### 方式一：通过 UPM Git URL 安装（推荐）

1. 打开 Unity。
2. 进入 `Window -> Package Manager`。
3. 点击左上角 `+`。
4. 选择 `Add package from git URL...`。
5. 输入：

```text
https://github.com/open1harry/presetpro.git
```

也可以固定分支：

```text
https://github.com/open1harry/presetpro.git#main
```

### 方式二：手动安装（本地包）

1. 将仓库下载到本地。
2. 把目录放到项目的 `Packages/com.vfxskill.presetpro`。
3. 在项目 `Packages/manifest.json` 里添加（可选）：

```json
{
  "dependencies": {
    "com.vfxskill.presetpro": "file:com.vfxskill.presetpro"
  }
}
```

## 基本使用

1. 打开 `VFXSkill -> Preset Pro -> Settings`（英文）或 `VFXSkill -> 预设 Pro -> 设置`（中文）。
2. 确认预设根目录（默认 `Assets/PresetPro/Presets`）。
3. 如有需要，设置 `GameObject` 菜单根节点名称。
4. 在预设目录下按文件夹创建分类并放入 Prefab。
5. 在设置中可编辑分类别名，并通过上下按钮调整分类顺序。
6. 点击 `Generate Menu` 或 `生成菜单` 生成菜单。
7. 在 Hierarchy 选中对象后按 `Shift + V`，快速保存为预设。

## 菜单说明

### 顶级工具菜单

- 中文：`VFXSkill -> 预设 Pro -> 工具介绍 / 设置 / 快速添加 / 生成菜单`
- 英文：`VFXSkill -> Preset Pro -> Introduction / Settings / Quick Add / Generate Menu`

### GameObject 菜单

生成后的实例化入口位于：

```text
GameObject/<自定义名称>/<分类>/<Prefab>
```

其中 `<自定义名称>` 默认为 `Preset Pro`，可在设置面板中修改。

## 目录结构

- `package.json`
- `Editor/`

## 场景建议（特效师）

- 按效果类型建分类：`Hit`、`Explosion`、`Trail`、`Buff`、`UIFx`
- 用快捷保存把调好的粒子对象沉淀为标准预设
- 通过 `GameObject` 原生菜单快速复用，减少重复搭建
- 作为个人/团队特效资产库，便于长期维护与备份
