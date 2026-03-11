# 📚 Documentation Quick Reference

This project includes comprehensive guides for different aspects. Pick what you need!

## 📖 Available Documentation

### 1. **CACHING_GUIDE.md** ⭐ (Interview Ready!)
   - **Size**: 21 KB
   - **Best for**: Understanding caching concepts and implementation
   - **Contents**:
     - When to use caching (✅ DO's and ❌ DON'Ts)
     - Types of caching: In-Memory vs Distributed (Redis)
     - Complete implementation steps
     - All core caching methods with code examples
     - **3 Implementation Levels**:
       - 🔵 **BASIC**: Simple In-Memory Cache
       - 🟡 **MEDIUM**: Key-Based Distributed Cache
       - 🔴 **ADVANCED**: Multi-Level Cache & Patterns
     - Best practices and checklist
     - Interview tips and quick reference
   - **Use**: Self-contained reference before interviews!

---

### 2. **DOCKER.md** 🐳
   - **Best for**: Understanding containerization
   - **Contents**:
     - Quick start with docker-compose
     - Service configuration (PostgreSQL, Redis, App)
     - Port mappings and environment variables
     - Health checks and networking
     - Troubleshooting common issues
     - Commands for logs and management
   - **Use**: When deploying to Docker

---

### 3. **README.md** (This Project)
   - **Best for**: General project overview
   - **Contents**:
     - Project structure
     - Getting started
     - Running locally vs Docker

---

## 🎯 Quick Navigation

### For Interview Preparation
👉 **Start with**: `CACHING_GUIDE.md`
- Read sections 1-4 (When to use, Types, Implementation, Methods)
- Study the Code Examples (Basic → Medium → Advanced)
- Review Best Practices section
- Use Interview Tips for talking points
- Memorize the Quick Reference tables

### For Implementation
👉 **Start with**: `CACHING_GUIDE.md` → Code Examples → Check Project

### For Deployment
👉 **Start with**: `DOCKER.md` → Quick start

---

## 📊 What Each Guide Covers

| Topic | CACHING_GUIDE | DOCKER | README |
|-------|---------------|--------|--------|
| When to cache | ✅ | - | - |
| Cache types | ✅ | - | - |
| In-Memory cache | ✅ | - | - |
| Distributed cache | ✅ | - | - |
| Implementation | ✅ | - | - |
| Code examples | ✅ | - | - |
| Best practices | ✅ | - | - |
| Containerization | - | ✅ | - |
| Docker setup | - | ✅ | ✅ |
| Services config | - | ✅ | - |
| Project overview | - | - | ✅ |

---

## ⏱️ Reading Time

- **CACHING_GUIDE.md**: 20-30 minutes (complete)
- **Core sections**: 10 minutes (When to use + Types)
- **Code examples**: 10 minutes
- **Interview prep**: 15 minutes (skim + Quick Reference)

---

## 🔑 Key Takeaways

### Caching Types
| Type | Speed | Instances | Persistent | Use Case |
|------|-------|-----------|-----------|----------|
| In-Memory | ⚡⚡⚡ | Single | ❌ | Single server, fast access |
| Distributed | ⚡⚡ | Multi | ✅ | Multiple servers, shared data |

### Implementation Methods
```
In-Memory:
- _memoryCache.TryGetValue(key, out value)
- _memoryCache.Set(key, value, TimeSpan.FromMinutes(20))
- _memoryCache.Remove(key)

Distributed (Redis):
- await _cache.GetStringAsync(key)
- await _cache.SetStringAsync(key, value, options)
- await _cache.RemoveAsync(key)
```

### Expiration Policy
- **Short** (5-10 min): Frequently changing data
- **Medium** (20-30 min): Semi-static data (Our default!)
- **Long** (60+ min): Static reference data

---

## 🚀 Project Stack

- **Language**: C# (.NET 10)
- **Caching**:
  - In-Memory: `Microsoft.Extensions.Caching.Memory`
  - Distributed: `StackExchange.Redis`
- **Database**: PostgreSQL
- **Cache Store**: Redis
- **Containers**: Docker & Docker Compose

---

## 📝 Before Your Interview

1. ✅ Read `CACHING_GUIDE.md` completely
2. ✅ Understand both cache types and their differences
3. ✅ Review the 3 code examples (Basic → Medium → Advanced)
4. ✅ Know when to use caching and when NOT to
5. ✅ Memorize core methods and their signatures
6. ✅ Be ready to explain the project's caching strategy
7. ✅ Have this file open to quickly reference topics

---

**Pro Tip**: Open `CACHING_GUIDE.md` during your interview prep and review:
- "When to Use Caching" section (1 min)
- "Types of Caching" comparison table (2 min)
- One code example at your level (3-5 min)
- Quick Reference section (1 min)
= **Total: ~10 minutes to feel confident!** 💪

---

**Last Updated**: March 11, 2026
**Project**: CachingDemo
