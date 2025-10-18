<template>
  <div class="modal show d-block" tabindex="-1">
    <div class="modal-dialog">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">Return Equipment</h5>
          <button type="button" class="btn-close" @click="$emit('close')"></button>
        </div>
        
        <form @submit.prevent="handleSubmit">
          <div class="modal-body">
            <div class="mb-3">
              <label for="rental" class="form-label">Active Rental</label>
              <select class="form-select" id="rental" v-model="form.rentalId" required>
                <option value="">Select Rental</option>
                <option 
                  v-for="rental in activeRentals" 
                  :key="rental.id" 
                  :value="rental.id"
                >
                  {{ rental.equipmentName }} - Due: {{ formatDate(rental.dueDate) }}
                </option>
              </select>
            </div>
            
            <div class="mb-3">
              <label for="condition" class="form-label">Return Condition</label>
              <select class="form-select" id="condition" v-model="form.condition" required>
                <option value="">Select Condition</option>
                <option value="Excellent">Excellent</option>
                <option value="Good">Good</option>
                <option value="Fair">Fair</option>
                <option value="Poor">Poor</option>
              </select>
            </div>
            
            <div class="mb-3">
              <label for="notes" class="form-label">Return Notes</label>
              <textarea 
                class="form-control" 
                id="notes" 
                v-model="form.notes"
                rows="3"
                placeholder="Any notes about the return condition..."
              ></textarea>
            </div>
          </div>
          
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" @click="$emit('close')">
              Cancel
            </button>
            <button type="submit" class="btn btn-success" :disabled="loading">
              <span v-if="loading" class="spinner-border spinner-border-sm me-2"></span>
              Return Equipment
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { rentalService } from '@/services/rental'
import { formatDate } from '@/utils/date'

const emit = defineEmits(['close', 'success'])

const form = ref({
  rentalId: '',
  condition: '',
  notes: ''
})

const activeRentals = ref([])
const loading = ref(false)

onMounted(async () => {
  await loadActiveRentals()
})

const loadActiveRentals = async () => {
  try {
    activeRentals.value = await rentalService.getActive()
  } catch (error) {
    console.error('Failed to load active rentals:', error)
  }
}

const handleSubmit = async () => {
  loading.value = true
  try {
    await rentalService.return(form.value)
    emit('success')
  } catch (error) {
    console.error('Failed to return equipment:', error)
  } finally {
    loading.value = false
  }
}
</script>
