<script setup lang="ts">
import { ref } from 'vue'
import { useDashboardStore } from '../../stores/useDashboardStore'

const store = useDashboardStore()
const memberId = ref<number | ''>('')
const description = ref('')
const saving = ref(false)
const error = ref('')

const submit = async () => {
  if (!memberId.value || !description.value.trim()) {
    error.value = 'Pick a teammate and describe the goal.'
    return
  }
  saving.value = true
  error.value = ''
  try {
    await store.addGoal(Number(memberId.value), description.value.trim())
    description.value = ''
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Unable to add goal.'
  } finally {
    saving.value = false
  }
}
</script>

<template>
  <form class="card bg-base-100 shadow-sm" @submit.prevent="submit">
    <div class="card-body space-y-3">
      <h2 class="card-title">Add Goal</h2>
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
        <span class="label-text">Goal description</span>
        <input v-model="description" type="text" class="input input-bordered" maxlength="250" required />
      </label>

      <p v-if="error" class="text-sm text-error">{{ error }}</p>

      <div class="card-actions justify-end">
        <button class="btn btn-primary" type="submit" :disabled="saving">
          {{ saving ? 'Saving…' : 'Add Goal' }}
        </button>
      </div>
    </div>
  </form>
</template>


