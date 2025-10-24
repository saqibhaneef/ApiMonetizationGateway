# 🧩 API Monetization Gateway

This project is a **.NET 8 Web API** that demonstrates how to build an **API Monetization Gateway** —  
a system that controls, tracks, and bills API usage based on customer tiers.

It sits between external API users and internal services to:
- ✅ Authenticate and identify customers  
- ✅ Enforce **rate limits** and **monthly quotas**  
- ✅ Track every API request  
- ✅ Generate **monthly usage summaries**  
- ✅ Expose the entire solution through a **Docker container**

---

## 🧱 Features

| Feature | Description |
|----------|-------------|
| **Tier-based Access** | Supports Free and Pro plans with custom limits |
| **Rate Limiting** | Controls requests per second dynamically |
| **Monthly Quota** | Stops users once monthly request limit is reached |
| **Usage Tracking** | Logs all API calls with timestamp and endpoint |
| **Monthly Summary** | Aggregates usage for each customer automatically |
| **Docker Support** | Build, run, and test easily using Docker |

---

## 🐳 Run with Docker

### 1. Pull the image
You can pull the prebuilt image from Docker Hub:
```bash
docker pull saqbhaneef/apimonetizationgateway:latest