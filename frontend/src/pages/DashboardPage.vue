<script setup lang="ts">
import MemberCard from '../components/MemberCard.vue'
import StatsPanel from '../components/StatsPanel.vue'
import AddGoalForm from '../components/forms/AddGoalForm.vue'
import UpdateMoodForm from '../components/forms/UpdateMoodForm.vue'
import OfflineBanner from '../components/OfflineBanner.vue'
import { useDashboardStore } from '../stores/useDashboardStore'

const store = useDashboardStore()
const toggleGoal = (goalId: number, isCompleted: boolean) => store.toggleGoal(goalId, isCompleted)
</script>

<template>
  <main class="mx-auto flex max-w-6xl flex-col gap-6 p-6">
    <header class="flex flex-col gap-2">
      <h1 class="text-3xl font-bold">GoalTracker Daily Dashboard</h1>
      <p class="text-base-content/70">Stay aligned on goals and morale in a single glance.</p>
    </header>

    <OfflineBanner
      v-if="store.state.isOffline"
      :message="store.state.error || 'You are offline.'"
      @retry="store.retry"
    />

    <StatsPanel :stats="store.state.stats" />

    <section class="grid gap-4 md:grid-cols-2">
      <AddGoalForm />
      <UpdateMoodForm />
    </section>

    <section v-if="store.state.loading" class="flex items-center gap-2 text-base-content/70">
      <span class="loading loading-spinner loading-md" />
      Fetching latest dashboard…
    </section>

    <section class="grid gap-4 md:grid-cols-2">
      <MemberCard
        v-for="member in store.state.members"
        :key="member.id"
        :member="member"
        :on-toggle="toggleGoal"
      />
    </section>
  </main>
</template>


