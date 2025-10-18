<template>
  <div class="toast-container position-fixed top-0 end-0 p-3">
    <div 
      v-for="toast in toasts" 
      :key="toast.id"
      :class="['toast', `bg-${toast.type}`]"
      role="alert"
    >
      <div class="toast-header">
        <strong class="me-auto">{{ toast.title }}</strong>
        <button 
          type="button" 
          class="btn-close" 
          @click="removeToast(toast.id)"
        ></button>
      </div>
      <div class="toast-body">
        {{ toast.message }}
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'

const toasts = ref([])

const addToast = (title, message, type = 'success') => {
  const id = Date.now()
  toasts.value.push({ id, title, message, type })
  
  setTimeout(() => {
    removeToast(id)
  }, 5000)
}

const removeToast = (id) => {
  const index = toasts.value.findIndex(toast => toast.id === id)
  if (index > -1) {
    toasts.value.splice(index, 1)
  }
}

defineExpose({ addToast })
</script>
