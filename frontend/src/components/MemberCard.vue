<script setup lang="ts">
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

const props = defineProps<{
  member: Member
  onToggle: (goalId: number, isCompleted: boolean) => Promise<void>
}>()

const toggle = (goal: Goal) => props.onToggle(goal.id, !goal.isCompleted)
</script>

<template>
  <article class="card bg-base-100 shadow-sm">
    <div class="card-body gap-3">
      <header class="flex items-center justify-between">
        <div class="flex items-center gap-2">
          <span class="text-3xl">{{ props.member.emoji || '😐' }}</span>
          <h3 class="font-semibold text-lg">{{ props.member.name }}</h3>
        </div>
        <span class="badge badge-neutral">{{ props.member.completedCount }}/{{ props.member.totalCount }}</span>
      </header>

      <p v-if="props.member.totalCount === 0" class="text-sm text-base-content/70">
        {{ props.member.helperText }}
      </p>

      <ul class="space-y-2">
        <li
          v-for="goal in props.member.goals"
          :key="goal.id"
          class="flex items-center gap-3 p-2 rounded bg-base-200"
        >
          <input
            type="checkbox"
            class="checkbox checkbox-sm"
            :checked="goal.isCompleted"
            @change="toggle(goal)"
          />
          <span :class="goal.isCompleted ? 'line-through opacity-60' : ''">{{ goal.description }}</span>
        </li>
      </ul>
    </div>
  </article>
</template>


