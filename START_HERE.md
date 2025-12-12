# 🚀 BG3ModManager Avalonia Migration - START HERE

**You have 5 minutes? Read this.**
**You have 30 minutes? Read the linked documents.**
**You have 2 hours? Deep dive into the full guides.**

---

## What's Happening?

We're migrating BG3ModManager from **Windows-only WPF** to **cross-platform Avalonia** to support both Windows and Linux users.

**TL;DR:** Single codebase → Both platforms ✨

---

## Documentation Quick Links

### Pick Your Role 👇

**I'm a...**

<details>
<summary><b>Project Manager / Team Lead</b></summary>

**Time:** 20 minutes

1. Read: [TEAM_MIGRATION_SUMMARY.md](./TEAM_MIGRATION_SUMMARY.md)
   - Overview: Why migrate?
   - Timeline: 4-6 weeks
   - Success metrics: How we measure success

2. Review: [AVALONIA_MIGRATION_PLAN.md](./AVALONIA_MIGRATION_PLAN.md) (Sections 1, 10-13)
   - Phases & Milestones
   - Risk Assessment
   - Questions for discussion

3. Bookmark: [IMPLEMENTATION_CHECKLIST.md](./IMPLEMENTATION_CHECKLIST.md)
   - Use for sprint planning
   - Track progress
   - Assign tasks

</details>

<details>
<summary><b>Frontend/UI Developer</b></summary>

**Time:** 2-3 hours

1. Setup (30 mins):
   - Follow: [GETTING_STARTED_AVALONIA.md](./GETTING_STARTED_AVALONIA.md)
   - Get environment running
   - Verify build works

2. Learn (1.5 hours):
   - Read: [AVALONIA_DEVELOPER_GUIDE.md](./AVALONIA_DEVELOPER_GUIDE.md)
   - Understand architecture
   - Learn common tasks
   - Review styling system

3. Plan (30 mins):
   - Check: [IMPLEMENTATION_CHECKLIST.md](./IMPLEMENTATION_CHECKLIST.md) Phase 3
   - Understand what UI components need porting
   - Get ready to start work

4. Code & Reference:
   - Keep GUIDE bookmarked
   - Use CHECKLIST to track progress
   - Reference code examples daily

</details>

<details>
<summary><b>Backend/Core Developer</b></summary>

**Time:** 2-3 hours

1. Setup (30 mins):
   - Follow: [GETTING_STARTED_AVALONIA.md](./GETTING_STARTED_AVALONIA.md)
   - Get environment running
   - Verify build works

2. Learn (1.5 hours):
   - Read: [AVALONIA_DEVELOPER_GUIDE.md](./AVALONIA_DEVELOPER_GUIDE.md)
   - Focus on: Platform Abstraction Layer section
   - Understand Windows vs Linux implementations
   - Review service interfaces

3. Plan (30 mins):
   - Check: [IMPLEMENTATION_CHECKLIST.md](./IMPLEMENTATION_CHECKLIST.md) Phase 2
   - Understand core refactoring tasks
   - Plan platform service implementations

4. Code & Reference:
   - Keep GUIDE's Platform Abstraction section bookmarked
   - Use CHECKLIST for Phase 2 tracking
   - Create Windows/Linux service implementations

</details>

<details>
<summary><b>DevOps / Build Engineer</b></summary>

**Time:** 1-2 hours

1. Quick Read:
   - Read: [TEAM_MIGRATION_SUMMARY.md](./TEAM_MIGRATION_SUMMARY.md) (section: DevOps/Build)
   - Understand scope

2. Deep Dive:
   - Read: [AVALONIA_MIGRATION_PLAN.md](./AVALONIA_MIGRATION_PLAN.md) Phase 5
   - Build system changes
   - CI/CD workflows needed

3. Planning:
   - Check: [IMPLEMENTATION_CHECKLIST.md](./IMPLEMENTATION_CHECKLIST.md) Phase 5
   - Multi-platform builds
   - Release packaging

4. Reference:
   - Keep PLAN Phase 5 bookmarked
   - Use CHECKLIST for build tasks
   - Create GitHub Actions workflows

</details>

<details>
<summary><b>QA / Test Engineer</b></summary>

**Time:** 1-2 hours

1. Setup (30 mins):
   - Follow: [GETTING_STARTED_AVALONIA.md](./GETTING_STARTED_AVALONIA.md)
   - Get environment running
   - Verify build works

2. Learn (1 hour):
   - Read: [AVALONIA_DEVELOPER_GUIDE.md](./AVALONIA_DEVELOPER_GUIDE.md) (Testing Guide section)
   - Understand testing strategy
   - Common test scenarios

3. Planning (30 mins):
   - Check: [IMPLEMENTATION_CHECKLIST.md](./IMPLEMENTATION_CHECKLIST.md) (Testing Checklist sections)
   - Test scenarios per phase
   - Regression testing plan

4. Testing:
   - Keep GUIDE Testing section bookmarked
   - Use CHECKLIST testing scenarios
   - Test on Windows and Linux
   - Track issues

</details>

<details>
<summary><b>New Team Member / Contributor</b></summary>

**Time:** 1-2 hours

1. **First:** Read this (5 mins) ← You're here!

2. **Second:** [GETTING_STARTED_AVALONIA.md](./GETTING_STARTED_AVALONIA.md) (30 mins)
   - Install prerequisites
   - Build project
   - Verify everything works

3. **Third:** [AVALONIA_DEVELOPER_GUIDE.md](./AVALONIA_DEVELOPER_GUIDE.md) (45 mins)
   - Learn architecture
   - Understand project structure
   - See code examples

4. **Finally:** Choose your role above and follow their path! 👆

</details>

---

## 30-Second Version

**Current:** WPF (Windows only) → **New:** Avalonia (Windows + Linux)

**Why?** Linux users want to use BG3ModManager.

**How?** 6 phases over 4-6 weeks:
1. Create Avalonia project
2. Refactor core for cross-platform
3. Port UI from XAML to AXAML
4. Handle platform-specific stuff (file dialogs, etc.)
5. Update build scripts
6. Test & release

**Your role?** Check the boxes above ↑

---

## Files at a Glance

| File | Size | Purpose | Audience |
|------|------|---------|----------|
| **START_HERE.md** | 📄 This file | Quick orientation | Everyone |
| **TEAM_MIGRATION_SUMMARY.md** | 11 KB | Executive overview | Leads, all roles |
| **AVALONIA_MIGRATION_PLAN.md** | 19 KB | Detailed strategy | Leads, developers |
| **AVALONIA_DEVELOPER_GUIDE.md** | 24 KB | Daily reference | Developers |
| **GETTING_STARTED_AVALONIA.md** | 14 KB | Setup guide | New members |
| **IMPLEMENTATION_CHECKLIST.md** | 19 KB | Task tracking | Everyone implementing |
| **MIGRATION_DOCS_README.md** | 6 KB | Doc index | Reference |

---

## Next Steps (Choose One)

### ✅ I want to understand the big picture
→ Read: [TEAM_MIGRATION_SUMMARY.md](./TEAM_MIGRATION_SUMMARY.md)

### ✅ I want to set up and start coding
→ Read: [GETTING_STARTED_AVALONIA.md](./GETTING_STARTED_AVALONIA.md)

### ✅ I want detailed implementation guides
→ Read: [AVALONIA_DEVELOPER_GUIDE.md](./AVALONIA_DEVELOPER_GUIDE.md)

### ✅ I want to plan sprints and track progress
→ Read: [IMPLEMENTATION_CHECKLIST.md](./IMPLEMENTATION_CHECKLIST.md)

### ✅ I want complete strategic context
→ Read: [AVALONIA_MIGRATION_PLAN.md](./AVALONIA_MIGRATION_PLAN.md)

### ✅ I'm unsure where to start
→ Check the role boxes above ↑

---

## One-Minute FAQ

**Q: Will the app work on Linux after migration?**
A: Yes! That's the whole point.

**Q: Will we have to maintain two separate codebases?**
A: No. One Avalonia codebase for both platforms.

**Q: How long will this take?**
A: 4-6 weeks with a small team.

**Q: Will it break anything?**
A: Core logic (business) stays the same. Only UI changes (from WPF → Avalonia).

**Q: Can macOS support come later?**
A: Yes! Avalonia supports it, so adding macOS later would be minimal effort.

**Q: What if I have more questions?**
A: Check [TEAM_MIGRATION_SUMMARY.md](./TEAM_MIGRATION_SUMMARY.md) FAQ section, or ask the team!

---

## Key Contacts

**Project Lead:** [Name to be filled in]
**Lead Developer:** [Name to be filled in]
**DevOps:** [Name to be filled in]

Need help? Ask in the Discord: https://discord.gg/j5gp6MD

---

## Success = 🎉

When we're done, users will be able to:
- ✅ Download BG3ModManager for Windows
- ✅ Download BG3ModManager for Linux
- ✅ Run the exact same features on both
- ✅ Have a smooth user experience on their OS

---

## Status

| Item | Status |
|------|--------|
| Strategy & Planning | ✅ Complete |
| Documentation | ✅ Complete |
| Team Training Materials | ✅ Complete |
| Implementation | 🔄 Ready to Start |

**Current Phase:** 🎯 Awaiting Phase 1 Start

---

## Let's Go! 🚀

Pick your role above, follow the recommended reading, and reach out if you need anything.

**The BG3ModManager Linux release is coming. Let's make it happen!**

---

**Last Updated:** December 11, 2025
**Documentation Status:** ✅ Ready for Review

