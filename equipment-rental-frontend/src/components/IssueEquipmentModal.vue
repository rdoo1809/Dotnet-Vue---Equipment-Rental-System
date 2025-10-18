<template>
  <div class="modal show d-block" tabindex="-1">
    <div class="modal-dialog">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">Issue Equipment</h5>
          <button type="button" class="btn-close" @click="$emit('close')"></button>
        </div>
        
        <form @submit.prevent="handleSubmit">
          <div class="modal-body">
            <div class="mb-3">
              <label for="equipment" class="form-label">Equipment</label>
              <select class="form-select" id="equipment" v-model="form.equipmentId" required>
                <option value="">Select Equipment</option>
                <option 
                  v-for="equipment in availableEquipment" 
                  :key="equipment.id" 
                  :value="equipment.id"
                >
                  {{ equipment.name }} - {{ equipment.category }}
                </option>
              </select>
            </div>
            
            <div v-if="isAdmin" class="mb-3">
              <label for="customer" class="form-label">Customer</label>
              <select class="form-select" id="customer" v-model="form.customerId" required>
                <option value="">Select Customer</option>
                <option 
                  v-for="customer in customers" 
                  :key="customer.id" 
                  :value="customer.id"
                >
                  {{ customer.userName }} ({{ customer.email }})
                </option>
              </select>
            </div>
            
            <div class="mb-3">
              <label for="dueDate" class="form-label">Due Date</label>
              <input 
                type="date" 
                class="form-control" 
                id="dueDate" 
                v-model="form.dueDate"
                required
              />
            </div>
          </div>
          
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" @click="$emit('close')">
              Cancel
            </button>
            <button type="submit" class="btn btn-primary" :disabled="loading">
              <span v-if="loading" class="spinner-border spinner-border-sm me-2"></span>
              Issue Equipment
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
  
  <!-- Error Modal -->
  <ErrorModal 
    v-if="showErrorModal"
    :error-title="errorTitle"
    :error-message="errorMessage"
    @close="showErrorModal = false"
  />
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { equipmentService } from '@/services/equipment'
import { customerService } from '@/services/customer'
import { rentalService } from '@/services/rental'
import ErrorModal from '@/components/ErrorModal.vue'

const authStore = useAuthStore()
const isAdmin = computed(() => authStore.user?.role === 'Admin')

const emit = defineEmits(['close', 'success'])

const form = ref({
  equipmentId: '',
  customerId: isAdmin.value ? '' : authStore.user?.id,
  dueDate: ''
})

const availableEquipment = ref([])
const customers = ref([])
const loading = ref(false)

// Error handling
const showErrorModal = ref(false)
const errorTitle = ref('')
const errorMessage = ref('')

onMounted(async () => {
  await loadAvailableEquipment()
  if (isAdmin.value) {
    await loadCustomers()
  }
})

const loadAvailableEquipment = async () => {
  try {
    availableEquipment.value = await equipmentService.getAvailable()
  } catch (error) {
    console.error('Failed to load available equipment:', error)
    showError('Failed to Load Equipment', 'Unable to load available equipment. Please try again.')
  }
}

const loadCustomers = async () => {
  try {
    customers.value = await customerService.getAll()
  } catch (error) {
    console.error('Failed to load customers:', error)
    showError('Failed to Load Customers', 'Unable to load customer list. Please try again.')
  }
}

const handleSubmit = async () => {
  loading.value = true
  try {
    await rentalService.issue(form.value)
    emit('success')
  } catch (error) {
    console.error('Failed to issue equipment:', error)
    
    // Extract error message from response
    let errorMsg = 'An unexpected error occurred. Please try again.'
    if (error.response?.data) {
      if (typeof error.response.data === 'string') {
        errorMsg = error.response.data
      } else if (error.response.data.message) {
        errorMsg = error.response.data.message
      } else if (error.response.data.title) {
        errorMsg = error.response.data.title
      }
    } else if (error.message) {
      errorMsg = error.message
    }
    
    showError('Failed to Issue Equipment', errorMsg)
  } finally {
    loading.value = false
  }
}

const showError = (title, message) => {
  errorTitle.value = title
  errorMessage.value = message
  showErrorModal.value = true
}
</script>
