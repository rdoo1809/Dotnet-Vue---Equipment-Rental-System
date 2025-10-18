<template>
  <div v-if="rental">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h1>Rental Details</h1>
      <div>
        <button 
          v-if="rental.status === 'Active'"
          @click="extendRental" 
          class="btn btn-warning me-2"
        >
          Extend Rental
        </button>
        <button 
          v-if="rental.status === 'Active'"
          @click="returnRental" 
          class="btn btn-success me-2"
        >
          Return Equipment
        </button>
        <button 
          v-if="isAdmin && rental.status === 'Overdue'"
          @click="forceReturn" 
          class="btn btn-danger"
        >
          Force Return
        </button>
      </div>
    </div>
    
    <div class="row">
      <div class="col-md-8">
        <div class="card">
          <div class="card-header">
            <h5>Rental Information</h5>
          </div>
          <div class="card-body">
            <div class="row">
              <div class="col-md-6">
                <p><strong>Equipment:</strong> {{ rental.equipmentName }}</p>
                <p><strong>Customer:</strong> {{ rental.customerName }}</p>
                <p><strong>Issue Date:</strong> {{ formatDate(rental.issuedAt) }}</p>
              </div>
              <div class="col-md-6">
                <p><strong>Due Date:</strong> {{ formatDate(rental.dueDate) }}</p>
                <p><strong>Status:</strong> 
                  <span :class="getStatusClass(rental.status)">
                    {{ rental.status }}
                  </span>
                </p>
                <p v-if="rental.returnedAt"><strong>Returned:</strong> {{ formatDate(rental.returnedAt) }}</p>
              </div>
            </div>
          </div>
        </div>
        
        <!-- Rental Notes -->
        <div v-if="rental.notes" class="card mt-4">
          <div class="card-header">
            <h5>Notes</h5>
          </div>
          <div class="card-body">
            <p>{{ rental.notes }}</p>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Extend Rental Modal -->
    <ExtendRentalModal 
      v-if="showExtendModal"
      :rental="rental"
      @close="showExtendModal = false"
      @success="handleExtendSuccess"
    />
    
    <!-- Return Equipment Modal -->
    <ReturnEquipmentModal 
      v-if="showReturnModal"
      @close="showReturnModal = false"
      @success="handleReturnSuccess"
    />
    
    <!-- Force Return Modal -->
    <ConfirmationModal
      v-if="showForceReturnModal"
      title="Force Return Equipment"
      message="Are you sure you want to force return this equipment?"
      @confirm="confirmForceReturn"
      @cancel="showForceReturnModal = false"
    />
  </div>
  
  <div v-else class="text-center">
    <div class="spinner-border" role="status">
      <span class="visually-hidden">Loading...</span>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { rentalService } from '@/services/rental'
import { formatDate } from '@/utils/date'
import ExtendRentalModal from '@/components/ExtendRentalModal.vue'
import ReturnEquipmentModal from '@/components/ReturnEquipmentModal.vue'
import ConfirmationModal from '@/components/ConfirmationModal.vue'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const rental = ref(null)
const showExtendModal = ref(false)
const showReturnModal = ref(false)
const showForceReturnModal = ref(false)

const isAdmin = computed(() => authStore.user?.role === 'Admin')

onMounted(async () => {
  await loadRental()
})

const loadRental = async () => {
  try {
    rental.value = await rentalService.getById(route.params.id)
  } catch (error) {
    console.error('Failed to load rental:', error)
    router.push('/rentals')
  }
}

const getStatusClass = (status) => {
  const classes = {
    'Active': 'badge bg-success',
    'Completed': 'badge bg-primary',
    'Overdue': 'badge bg-danger',
    'Cancelled': 'badge bg-secondary'
  }
  return classes[status] || 'badge bg-secondary'
}

const extendRental = () => {
  showExtendModal.value = true
}

const returnRental = () => {
  showReturnModal.value = true
}

const forceReturn = () => {
  showForceReturnModal.value = true
}

const handleExtendSuccess = () => {
  showExtendModal.value = false
  loadRental()
}

const handleReturnSuccess = () => {
  showReturnModal.value = false
  loadRental()
}

const confirmForceReturn = async () => {
  try {
    await rentalService.cancel(rental.value.id)
    showForceReturnModal.value = false
    router.push('/rentals')
  } catch (error) {
    console.error('Failed to force return:', error)
  }
}
</script>
