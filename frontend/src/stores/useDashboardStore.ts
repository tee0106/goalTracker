import { reactive } from 'vue'
import { useApi } from '../composables/useApi'

type Goal = { id: number; description: string; isCompleted: boolean }
type Member = {
  id: number
  name: string
  emoji: string
  goals: Goal[]
  completedCount: number
  totalCount: number
  helperText: string
}
type Stats = { completionPercent: number; happyCount: number; neutralCount: number; stressedCount: number }
type DashboardResponse = { members: Member[]; stats: Stats }
type GoalResponse = { member: Member; stats: Stats }
type MoodResponse = { memberId: number; emoji: string; stats: Stats }

const api = useApi()
const defaults: Stats = { completionPercent: 0, happyCount: 0, neutralCount: 0, stressedCount: 0 }
const state = reactive({
  members: [] as Member[],
  stats: defaults,
  loading: false,
  error: '',
  isOffline: false,
  lastUpdated: 0,
})

let pollHandle: number | null = null

const setMembers = (members: Member[]) => {
  state.members = members
}

const setStats = (stats: Stats) => {
  state.stats = stats
}

const mergeMember = (member: Member) => {
  const idx = state.members.findIndex((m) => m.id === member.id)
  if (idx === -1) {
    state.members.push(member)
  } else {
    state.members[idx] = member
  }
}

const loadDashboard = async () => {
  try {
    state.loading = true
    const data = await api.get<DashboardResponse>('/api/dashboard')
    setMembers(data.members)
    setStats(data.stats)
    state.error = ''
    state.isOffline = false
    state.lastUpdated = Date.now()
  } catch (error) {
    state.error = error instanceof Error ? error.message : 'Unable to load dashboard'
    state.isOffline = true
    throw error
  } finally {
    state.loading = false
  }
}

const ensurePolling = () => {
  if (pollHandle) return
  pollHandle = window.setInterval(() => {
    loadDashboard().catch(() => undefined)
  }, 15000)
}

const addGoal = async (memberId: number, description: string) => {
  const response = await api.post<GoalResponse>('/api/goals', { memberId, description })
  mergeMember(response.member)
  setStats(response.stats)
}

const toggleGoal = async (goalId: number, isCompleted: boolean) => {
  const response = await api.patch<GoalResponse>(`/api/goals/${goalId}`, { isCompleted })
  mergeMember(response.member)
  setStats(response.stats)
}

const updateMood = async (memberId: number, emoji: string) => {
  const response = await api.post<MoodResponse>('/api/moods', { memberId, emoji })
  const member = state.members.find((m) => m.id === memberId)
  if (member) {
    member.emoji = response.emoji
  }
  setStats(response.stats)
}

loadDashboard().catch(() => undefined)
ensurePolling()

export function useDashboardStore() {
  return {
    state,
    loadDashboard,
    retry: () => loadDashboard(),
    addGoal,
    toggleGoal,
    updateMood,
  }
}


