<script setup lang="ts">
import { ref } from 'vue'
import { useDashboardStore } from '../../stores/useDashboardStore'

const store = useDashboardStore()
const memberId = ref<number | ''>('')
const emoji = ref('😀')
const saving = ref(false)
const error = ref('')
const emojis = ['😀', '😊', '😐', '😞', '😤']

const submit = async () => {
  if (!memberId.value) {
    error.value = 'Select a teammate.'
    return
  }
  saving.value = true
  error.value = ''
  try {
    await store.updateMood(Number(memberId.value), emoji.value)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Unable to update mood.'
  } finally {
    saving.value = false
  }
}
</script>

<template>
  <form class="card bg-base-100 shadow-sm" @submit.prevent="submit">
    <div class="card-body space-y-3">
      <h2 class="card-title">Update Mood</h2>
      <label class="form-control w-full">
        <span class="label-text">Teammate</span>
        <select v-model="memberId" class="select select-bordered" required>
          <option value="" disabled>Select member</option>
          <option v-for="member in store.state.members" :key="member.id" :value="member.id">
            {{ member.name }}
          </option>
        </select>
      </label>

      <label class="form-control">
        <span class="label-text">Emoji</span>
        <div class="flex gap-2">
          <label
            v-for="option in emojis"
            :key="option"
            class="btn btn-sm"
            :class="emoji === option ? 'btn-secondary' : 'btn-outline'"
          >
            <input v-model="emoji" type="radio" class="hidden" :value="option" />
            {{ option }}
          </label>
        </div>
      </label>

      <p v-if="error" class="text-sm text-error">{{ error }}</p>

      <div class="card-actions justify-end">
        <button class="btn btn-secondary" type="submit" :disabled="saving">
          {{ saving ? 'Saving…' : 'Save Mood' }}
        </button>
      </div>
    </div>
  </form>
</template>


