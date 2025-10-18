<template>
  <div class="modal show d-block" tabindex="-1">
    <div class="modal-dialog">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">Extend Rental</h5>
          <button type="button" class="btn-close" @click="$emit('close')"></button>
        </div>
        
        <form @submit.prevent="handleSubmit">
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">Equipment</label>
              <p class="form-control-plaintext">{{ rental?.equipmentName }}</p>
            </div>
            
            <div class="mb-3">
              <label class="form-label">Current Due Date</label>
              <p class="form-control-plaintext">{{ formatDate(rental?.dueDate) }}</p>
            </div>
            
            <div class="mb-3">
              <label for="newDueDate" class="form-label">New Due Date</label>
              <input 
                type="date" 
                class="form-control" 
                id="newDueDate" 
                v-model="form.newDueDate"
                :min="minDate"
                required
              />
            </div>
            
            <div class="mb-3">
              <label for="reason" class="form-label">Extension Reason</label>
              <textarea 
                class="form-control" 
                id="reason" 
                v-model="form.reason"
                rows="3"
                placeholder="Reason for extending the rental..."
                required
              ></textarea>
            </div>
          </div>
          
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" @click="$emit('close')">
              Cancel
            </button>
            <button type="submit" class="btn btn-warning" :disabled="loading">
              <span v-if="loading" class="spinner-border spinner-border-sm me-2"></span>
              Extend Rental
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { rentalService } from '@/services/rental'
import { formatDate } from '@/utils/date'

const props = defineProps({
  rental: {
    type: Object,
    required: true
  }
})

const emit = defineEmits(['close', 'success'])

const form = ref({
  newDueDate: '',
  reason: ''
})

const loading = ref(false)

const minDate = computed(() => {
  if (!props.rental?.dueDate) return ''
  const tomorrow = new Date(props.rental.dueDate)
  tomorrow.setDate(tomorrow.getDate() + 1)
  return tomorrow.toISOString().split('T')[0]
})

onMounted(() => {
  if (props.rental?.dueDate) {
    const tomorrow = new Date(props.rental.dueDate)
    tomorrow.setDate(tomorrow.getDate() + 1)
    form.value.newDueDate = tomorrow.toISOString().split('T')[0]
  }
})

const handleSubmit = async () => {
  loading.value = true
  try {
    await rentalService.extend(props.rental.id, form.value)
    emit('success')
  } catch (error) {
    console.error('Failed to extend rental:', error)
  } finally {
    loading.value = false
  }
}
</script>
