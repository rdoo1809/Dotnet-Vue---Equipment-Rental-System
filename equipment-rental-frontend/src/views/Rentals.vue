<template>
  <div>
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h1>Rentals</h1>
      <div>
        <button 
          @click="showIssueModal = true" 
          class="btn btn-primary me-2"
        >
          Issue Equipment
        </button>
        <button 
          @click="showReturnModal = true" 
          class="btn btn-success"
        >
          Return Equipment
        </button>
      </div>
    </div>
    
    <!-- Filter Tabs -->
    <ul class="nav nav-tabs mb-4">
      <li class="nav-item">
        <button 
          :class="['nav-link', { active: activeTab === 'all' }]"
          @click="setActiveTab('all')"
        >
          All Rentals
        </button>
      </li>
      <li class="nav-item">
        <button 
          :class="['nav-link', { active: activeTab === 'active' }]"
          @click="setActiveTab('active')"
        >
          Active
        </button>
      </li>
      <li class="nav-item">
        <button 
          :class="['nav-link', { active: activeTab === 'completed' }]"
          @click="setActiveTab('completed')"
        >
          Completed
        </button>
      </li>
      <li class="nav-item">
        <button 
          :class="['nav-link', { active: activeTab === 'overdue' }]"
          @click="setActiveTab('overdue')"
        >
          Overdue
        </button>
      </li>
    </ul>
    
    <!-- Rentals Table -->
    <div class="card">
      <div class="card-body">
        <div class="table-responsive">
          <table class="table table-striped">
            <thead>
              <tr>
                <th>Equipment</th>
                <th>Customer</th>
                <th>Issue Date</th>
                <th>Due Date</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr 
                v-for="rental in filteredRentals" 
                :key="rental.id"
                :class="{ 'table-danger': rental.status === 'Overdue' }"
              >
                <td>{{ rental.equipmentName }}</td>
                <td>{{ rental.customerName }}</td>
                <td>{{ formatDate(rental.issuedAt) }}</td>
                <td>{{ formatDate(rental.dueDate) }}</td>
                <td>
                  <span :class="getStatusClass(rental.status)">
                    {{ rental.status }}
                  </span>
                </td>
                <td>
                  <div class="btn-group" role="group">
                    <router-link 
                      :to="`/rental/${rental.id}`" 
                      class="btn btn-sm btn-outline-primary"
                    >
                      View
                    </router-link>
                    <button 
                      v-if="rental.status === 'Active'"
                      @click="extendRental(rental)" 
                      class="btn btn-sm btn-outline-warning"
                    >
                      Extend
                    </button>
                    <button 
                      v-if="rental.status === 'Active'"
                      @click="returnRental(rental)" 
                      class="btn btn-sm btn-outline-success"
                    >
                      Return
                    </button>
                    <button 
                      v-if="isAdmin && rental.status === 'Overdue'"
                      @click="forceReturn(rental)" 
                      class="btn btn-sm btn-danger"
                    >
                      Force Return
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
    
    <!-- Issue Equipment Modal -->
    <IssueEquipmentModal 
      v-if="showIssueModal"
      @close="showIssueModal = false"
      @success="handleIssueSuccess"
    />
    
    <!-- Return Equipment Modal -->
    <ReturnEquipmentModal 
      v-if="showReturnModal"
      @close="showReturnModal = false"
      @success="handleReturnSuccess"
    />
    
    <!-- Extend Rental Modal -->
    <ExtendRentalModal 
      v-if="showExtendModal"
      :rental="extendingRental"
      @close="showExtendModal = false"
      @success="handleExtendSuccess"
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
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { rentalService } from '@/services/rental'
import { formatDate } from '@/utils/date'
import IssueEquipmentModal from '@/components/IssueEquipmentModal.vue'
import ReturnEquipmentModal from '@/components/ReturnEquipmentModal.vue'
import ExtendRentalModal from '@/components/ExtendRentalModal.vue'
import ConfirmationModal from '@/components/ConfirmationModal.vue'

const authStore = useAuthStore()
const isAdmin = computed(() => authStore.user?.role === 'Admin')

const activeTab = ref('all')
const allRentals = ref([])
const activeRentals = ref([])
const completedRentals = ref([])
const overdueRentals = ref([])

const showIssueModal = ref(false)
const showReturnModal = ref(false)
const showExtendModal = ref(false)
const showForceReturnModal = ref(false)
const extendingRental = ref(null)
const forceReturnRental = ref(null)

const filteredRentals = computed(() => {
  switch (activeTab.value) {
    case 'active': return activeRentals.value
    case 'completed': return completedRentals.value
    case 'overdue': return overdueRentals.value
    default: return allRentals.value
  }
})

onMounted(() => {
  loadRentals()
})

const loadRentals = async () => {
  try {
    const [all, active, completed, overdue] = await Promise.all([
      rentalService.getAll(),
      rentalService.getActive(),
      rentalService.getCompleted(),
      rentalService.getOverdue()
    ])
    
    allRentals.value = all
    activeRentals.value = active
    completedRentals.value = completed
    overdueRentals.value = overdue
  } catch (error) {
    console.error('Failed to load rentals:', error)
  }
}

const setActiveTab = (tab) => {
  activeTab.value = tab
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

const extendRental = (rental) => {
  extendingRental.value = rental
  showExtendModal.value = true
}

const returnRental = (rental) => {
  // Implementation for return rental
}

const forceReturn = (rental) => {
  forceReturnRental.value = rental
  showForceReturnModal.value = true
}

const handleIssueSuccess = () => {
  showIssueModal.value = false
  loadRentals()
}

const handleReturnSuccess = () => {
  showReturnModal.value = false
  loadRentals()
}

const handleExtendSuccess = () => {
  showExtendModal.value = false
  extendingRental.value = null
  loadRentals()
}

const confirmForceReturn = async () => {
  try {
    await rentalService.cancel(forceReturnRental.value.id)
    showForceReturnModal.value = false
    forceReturnRental.value = null
    loadRentals()
  } catch (error) {
    console.error('Failed to force return:', error)
  }
}
</script>
