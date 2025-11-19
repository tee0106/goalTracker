<script setup lang="ts">
defineProps<{ message: string }>()
const emit = defineEmits<{ retry: [] }>()
</script>

<template>
  <div class="alert alert-warning shadow-sm">
    <span>{{ message }}</span>
    <button class="btn btn-sm" type="button" @click="emit('retry')">Retry</button>
  </div>
</template>


