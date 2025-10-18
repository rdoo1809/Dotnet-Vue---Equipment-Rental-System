<template>
  <div class="modal show d-block" tabindex="-1">
    <div class="modal-dialog">
      <div class="modal-content">
        <div class="modal-header bg-danger text-white">
          <h5 class="modal-title">
            <i class="bi bi-exclamation-triangle-fill me-2"></i>
            Error
          </h5>
          <button type="button" class="btn-close btn-close-white" @click="$emit('close')"></button>
        </div>
        
        <div class="modal-body">
          <div class="alert alert-danger mb-0">
            <strong>{{ errorTitle }}</strong>
            <p class="mb-0 mt-2">{{ errorMessage }}</p>
          </div>
        </div>
        
        <div class="modal-footer">
          <button type="button" class="btn btn-danger" @click="$emit('close')">
            Close
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
const props = defineProps({
  errorTitle: {
    type: String,
    default: 'An error occurred'
  },
  errorMessage: {
    type: String,
    required: true
  }
})

const emit = defineEmits(['close'])
</script>
