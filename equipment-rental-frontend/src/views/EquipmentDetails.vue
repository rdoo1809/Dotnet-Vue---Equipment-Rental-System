<template>
  <div v-if="equipment">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h1>{{ equipment.name }}</h1>
      <div>
        <button 
          v-if="isAdmin" 
          @click="editEquipment" 
          class="btn btn-secondary me-2"
        >
          Edit
        </button>
        <button 
          v-if="isAdmin" 
          @click="deleteEquipment" 
          class="btn btn-danger me-2"
        >
          Delete
        </button>
        <button 
          v-if="canIssue" 
          @click="issueEquipment" 
          class="btn btn-primary"
        >
          Issue Equipment
        </button>
      </div>
    </div>
    
    <div class="row">
      <div class="col-md-8">
        <div class="card">
          <div class="card-header">
            <h5>Equipment Details</h5>
          </div>
          <div class="card-body">
            <div class="row">
              <div class="col-md-6">
                <p><strong>Name:</strong> {{ equipment.name }}</p>
                <p><strong>Category:</strong> {{ equipment.category }}</p>
                <p><strong>Condition:</strong> {{ equipment.condition }}</p>
              </div>
              <div class="col-md-6">
                <p><strong>Status:</strong> 
                  <span :class="getStatusClass(equipment.status)">
                    {{ equipment.status }}
                  </span>
                </p>
                <p><strong>Description:</strong> {{ equipment.description }}</p>
              </div>
            </div>
          </div>
        </div>
        
        <!-- Rental History (Admin only) -->
        <div v-if="isAdmin" class="card mt-4">
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
                  <span>{{ rental.customerName }}</span>
                  <span class="text-muted">{{ formatDate(rental.issuedAt) }} - {{ formatDate(rental.returnedAt) }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Issue Equipment Modal -->
    <IssueEquipmentModal 
      v-if="showIssueModal"
      :equipment="equipment"
      @close="showIssueModal = false"
      @success="handleIssueSuccess"
    />
    
    <!-- Edit Modal -->
    <EquipmentModal 
      v-if="showEditModal"
      :equipment="equipment"
      @close="showEditModal = false"
      @save="handleEditSave"
    />
    
    <!-- Delete Confirmation Modal -->
    <ConfirmationModal
      v-if="showDeleteModal"
      title="Delete Equipment"
      message="Are you sure you want to delete this equipment?"
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
import { equipmentService } from '@/services/equipment'
import { rentalService } from '@/services/rental'
import { formatDate } from '@/utils/date'
import IssueEquipmentModal from '@/components/IssueEquipmentModal.vue'
import EquipmentModal from '@/components/EquipmentModal.vue'
import ConfirmationModal from '@/components/ConfirmationModal.vue'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const equipment = ref(null)
const rentalHistory = ref([])
const showIssueModal = ref(false)
const showEditModal = ref(false)
const showDeleteModal = ref(false)

const isAdmin = computed(() => authStore.user?.role === 'Admin')
const canIssue = computed(() => equipment.value?.status === 'Available' && !isAdmin.value)

onMounted(async () => {
  await loadEquipment()
  if (isAdmin.value) {
    await loadRentalHistory()
  }
})

const loadEquipment = async () => {
  try {
    equipment.value = await equipmentService.getById(route.params.id)
  } catch (error) {
    console.error('Failed to load equipment:', error)
    router.push('/equipment')
  }
}

const loadRentalHistory = async () => {
  try {
    rentalHistory.value = await rentalService.getEquipmentHistory(route.params.id)
  } catch (error) {
    console.error('Failed to load rental history:', error)
  }
}

const getStatusClass = (status) => {
  const classes = {
    'Available': 'badge bg-success',
    'Rented': 'badge bg-warning',
    'Maintenance': 'badge bg-danger'
  }
  return classes[status] || 'badge bg-secondary'
}

const issueEquipment = () => {
  showIssueModal.value = true
}

const editEquipment = () => {
  showEditModal.value = true
}

const deleteEquipment = () => {
  showDeleteModal.value = true
}

const handleIssueSuccess = () => {
  showIssueModal.value = false
  loadEquipment()
}

const handleEditSave = async (equipmentData) => {
  try {
    await equipmentService.update(equipment.value.id, equipmentData)
    showEditModal.value = false
    loadEquipment()
  } catch (error) {
    console.error('Failed to update equipment:', error)
  }
}

const confirmDelete = async () => {
  try {
    await equipmentService.delete(equipment.value.id)
    showDeleteModal.value = false
    router.push('/equipment')
  } catch (error) {
    console.error('Failed to delete equipment:', error)
  }
}
</script>
