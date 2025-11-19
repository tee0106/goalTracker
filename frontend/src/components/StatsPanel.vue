<script setup lang="ts">
type Stats = { completionPercent: number; happyCount: number; neutralCount: number; stressedCount: number }

defineProps<{ stats: Stats }>()
</script>

<template>
  <section class="card bg-primary text-primary-content">
    <div class="card-body">
      <h2 class="card-title">Team Pulse</h2>
      <div class="flex flex-wrap gap-6">
        <div>
          <p class="text-sm opacity-80">Completion</p>
          <p class="text-3xl font-bold">{{ stats.completionPercent.toFixed(1) }}%</p>
        </div>
        <div class="flex gap-4 text-sm">
          <div>
            <p class="opacity-80">Happy</p>
            <p class="text-xl font-semibold">{{ stats.happyCount }}</p>
          </div>
          <div>
            <p class="opacity-80">Neutral</p>
            <p class="text-xl font-semibold">{{ stats.neutralCount }}</p>
          </div>
          <div>
            <p class="opacity-80">Stressed</p>
            <p class="text-xl font-semibold">{{ stats.stressedCount }}</p>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>


