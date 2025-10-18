<template>
  <div v-if="customer">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h1>{{ customer.name }}</h1>
      <div>
        <button 
          v-if="isAdmin" 
          @click="editCustomer" 
          class="btn btn-secondary me-2"
        >
          Edit
        </button>
        <button 
          v-if="isAdmin" 
          @click="deleteCustomer" 
          class="btn btn-danger"
        >
          Delete
        </button>
      </div>
    </div>
    
    <div class="row">
      <div class="col-md-8">
        <div class="card">
          <div class="card-header">
            <h5>Customer Details</h5>
          </div>
          <div class="card-body">
            <div class="row">
              <div class="col-md-6">
                <p><strong>Name:</strong> {{ customer.name }}</p>
                <p><strong>Username:</strong> {{ customer.username }}</p>
                <p><strong>Role:</strong> 
                  <span :class="getRoleClass(customer.role)">
                    {{ customer.role }}
                  </span>
                </p>
              </div>
              <div class="col-md-6">
                <p><strong>Active Rental:</strong> 
                  <span v-if="customer.activeRental" class="text-warning">
                    {{ customer.activeRental.equipmentName }}
                  </span>
                  <span v-else class="text-muted">None</span>
                </p>
              </div>
            </div>
          </div>
        </div>
        
        <!-- Rental History -->
        <div class="card mt-4">
          <div class="card-header">
            <h5>Rental History</h5>
          </div>
          <div class="card-body">
            <div v-if="rentalHistory.length === 0" class="text-muted">
              No rental history available
            </div>
            <div v-else>
              <div v-for="rental in rentalHistory" :key="rental.id" class="border-bottom py-2">
                <div class="d-flex justify-content-between">
                  <span>{{ rental.equipmentName }}</span>
                  <span class="text-muted">{{ formatDate(rental.issuedAt) }} - {{ formatDate(rental.returnedAt) }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Edit Modal -->
    <CustomerModal 
      v-if="showEditModal"
      :customer="customer"
      @close="showEditModal = false"
      @save="handleEditSave"
    />
    
    <!-- Delete Confirmation Modal -->
    <ConfirmationModal
      v-if="showDeleteModal"
      title="Delete Customer"
      message="Are you sure you want to delete this customer and their history?"
      @confirm="confirmDelete"
      @cancel="showDeleteModal = false"
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
import { customerService } from '@/services/customer'
import { formatDate } from '@/utils/date'
import CustomerModal from '@/components/CustomerModal.vue'
import ConfirmationModal from '@/components/ConfirmationModal.vue'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const customer = ref(null)
const rentalHistory = ref([])
const showEditModal = ref(false)
const showDeleteModal = ref(false)

const isAdmin = computed(() => authStore.user?.role === 'Admin')

onMounted(async () => {
  await loadCustomer()
  await loadRentalHistory()
})

const loadCustomer = async () => {
  try {
    customer.value = await customerService.getById(route.params.id)
  } catch (error) {
    console.error('Failed to load customer:', error)
    router.push('/customers')
  }
}

const loadRentalHistory = async () => {
  try {
    rentalHistory.value = await customerService.getRentals(route.params.id)
  } catch (error) {
    console.error('Failed to load rental history:', error)
  }
}

const getRoleClass = (role) => {
  return role === 'Admin' ? 'badge bg-danger' : 'badge bg-primary'
}

const editCustomer = () => {
  showEditModal.value = true
}

const deleteCustomer = () => {
  showDeleteModal.value = true
}

const handleEditSave = async (customerData) => {
  try {
    await customerService.update(customer.value.id, customerData)
    showEditModal.value = false
    loadCustomer()
  } catch (error) {
    console.error('Failed to update customer:', error)
  }
}

const confirmDelete = async () => {
  try {
    await customerService.delete(customer.value.id)
    showDeleteModal.value = false
    router.push('/customers')
  } catch (error) {
    console.error('Failed to delete customer:', error)
  }
}
</script>
