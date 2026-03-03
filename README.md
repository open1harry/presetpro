# Preset Pro

`Preset Pro` 是一个 Unity 编辑器效率插件，用于快速保存、整理、备份 Prefab 预设，并自动生成原生菜单入口。

它特别适合特效师（VFX Artist）管理粒子系统资产：你可以把常用特效对象快速沉淀为可复用的预设库，按分类整理，随时一键实例化。

## 核心价值

- 快速保存和备份场景中的对象为 Prefab 预设
- 基于文件夹分类，天然支持批量导入导出
- 自动同步到 `GameObject/Preset Pro/...` 原生菜单
- 适合搭建个人粒子系统预设库、特效模板库

## 功能特性

- 文件夹即分类，Prefab 即选项
- `Tools -> Preset Pro -> Settings` 设置面板
- `Shift + V` 快捷保存（Quick Add）
- 自动生成 Unity `MenuItem` 菜单代码
- 首次打开自动显示工具介绍
- 默认中文界面（可切换英文）

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
3. 在项目 `Packages/manifest.json` 里添加：

```json
{
  "dependencies": {
    "com.vfxskill.presetpro": "file:com.vfxskill.presetpro"
  }
}
```

## 基本使用

1. 打开 `Tools -> Preset Pro -> Settings`。
2. 设置或确认预设根目录（默认 `Assets/PresetPro/Presets`）。
3. 在该目录下按文件夹创建分类并放入 Prefab。
4. 点击 `Generate/Refresh Menu` 生成原生菜单。
5. 在 Hierarchy 选中对象后按 `Shift + V`，快速保存为预设。

## 目录结构

- `package.json`
- `Editor/`

## 场景建议（特效师）

- 按效果类型建分类：`Hit`、`Explosion`、`Trail`、`Buff`、`UIFx`
- 用快捷保存把调好的粒子对象沉淀为标准预设
- 通过原生菜单快速复用，减少重复搭建
- 作为个人/团队特效资产库，便于长期维护与备份
