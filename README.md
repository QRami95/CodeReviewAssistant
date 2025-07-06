# 🤖 Code Review Assistant (.NET + Azure OpenAI + GitHub)

A lightweight AI-powered code review assistant for GitHub Pull Requests. It analyzes changed files using Azure OpenAI and leaves concise summary comments directly in the PR.

---

## 🚀 Features

- Integrates with GitHub PRs via GitHub REST API (Octokit)
- Uses Azure OpenAI for review generation (e.g., GPT-4 or GPT-3.5)
- Analyzes **all changed files**
- Adds **inline comments** to changed lines (on correct diff positions)
- Concise, high-level summary per file

---

## 🛠️ Tech Stack

- .NET 8
- Azure OpenAI SDK
- GitHub Octokit SDK
- Minimal API (ASP.NET Core)

---

## 📦 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- Azure subscription with OpenAI deployed
- GitHub Personal Access Token (with `repo` scope)

---

## ⚙️ Setup Instructions

### 1. 🔐 Generate a GitHub Token

1. Go to [GitHub Developer Settings → Tokens (classic)](https://github.com/settings/tokens)
2. Click **"Generate new token (classic)"**
3. Select:
   - **Expiration**: Choose what fits
   - **Scopes**: Check `repo`
4. Click **Generate Token** and copy it
5. Add the values into the `AppSettings.json`

Then in your shell:


### 2. 🤖 Create Azure OpenAI Deployment

To use Azure-hosted GPT models (like GPT-4 or GPT-3.5-turbo), follow these steps:

#### a. Create Azure OpenAI Resource

1. Go to [Azure Portal](https://portal.azure.com)
2. Click **Create a resource**
3. Search for **"Azure OpenAI"**
4. Create a new resource with the desired region (e.g., `East US`)
5. Wait for deployment to complete

#### b. Deploy a Model

1. Navigate to your **Azure OpenAI resource**
2. Go to the **Deployments** blade in the left panel
3. Click **+ Create**
4. Select a model:
   - `gpt-35-turbo` (for GPT-3.5)
   - `gpt-4` (may require approval)
5. Choose a **deployment name** (e.g., `code-review`)
6. Click **Create**

> 🔁 Note: It may take a few minutes for the deployment to become active.

---

### 3. 🔐 Get Azure OpenAI Credentials

1. From your Azure OpenAI resource:
   - Go to **Keys and Endpoint**
   - Copy your **Endpoint** and **Key**

2. Add the values into the `AppSettings.json`


## 📄 License

This project is licensed under the [MIT License](LICENSE).
