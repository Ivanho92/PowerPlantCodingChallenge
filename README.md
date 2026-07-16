# Power Plant Coding Challenge

## Overview

This project contains an ASP.NET Core API that calculates production plans for power plants.

Further details can be found at:
https://github.com/gem-spaas/powerplant-coding-challenge

---

## Prerequisites

The API can be launched either directly with the .NET SDK or using Docker Compose:

### Run with .NET SDK

Requires:

* .NET SDK 10.0 or later

### Run with Docker

Requires:

* Docker Desktop
* Docker Compose

---

# Running the API

From the repository root directory, there are two ways to start the API.

## Option 1 - Run with .NET

```bash
dotnet run --project PowerPlantCodingChallenge
```

## Option 2 - Run with Docker Compose

```bash
docker compose up -d
```

---

# API Endpoint

Once the API is running, the production plan endpoint is available at:

```
http://localhost:8888/productionplan
```

---

# Testing the API with predefined requests

The project contains predefined HTTP requests that can be executed directly from compatible HTTP clients (for example JetBrains Rider or Visual Studio).

Open:

```
PowerPlantCodingChallenge/PowerPlantCodingChallenge.http
```

and execute the provided requests against the running API.

---

# Logs

Log files are written to:

```
PowerPlantCodingChallenge/logs
```

Note: This folder is used both when running the API locally and when running it through Docker Compose.

---

# Running Unit Tests (optional)

To execute the test suite:

```bash
dotnet test
```