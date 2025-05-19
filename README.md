# Introduction
This is the API for the selfhosted Cookbook, build with C# and Entity Framework.
If you want to see the Frontend build with React and TypeScript please visit 
https://github.com/JGoseberg/CookBook_React.

# Features
- Dockerized
- CI/CD Based

# Getting Started
## Self-hosted
### Pre-Requirements
- Install Docker
- Clone Repository
```
git clone https://github.com/JGoseberg/CookBook_Api.git
```
- Build the Docker Image
```
docker build -t cookbook-api:latest .
```
- Run the Docker Image 
```
docker run -d \
--name cookbook-api \
-p 5046:8080 \
-e ASPNETCORE_URLS=http://0.0.0.0:8080 \
--restart unless-stopped \
cookbook-api:latest
```

# Roadmap
## 05/2025
- Development started

