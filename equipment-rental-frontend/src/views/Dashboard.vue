<template>
  <div>
    <h1 class="mb-4">Equipment Rental Dashboard</h1>
    <p class="text-muted mb-4">Welcome to the Equipment Rental Management System.</p>
    
    <!-- Stats Cards -->
    <div class="row mb-4">
      <div class="col-md-3">
        <div class="card text-white bg-primary">
          <div class="card-body">
            <h2 class="card-title">{{ stats.total }}</h2>
            <p class="card-text">Total Equipment</p>
            <router-link to="/equipment" class="btn btn-light">View All</router-link>
          </div>
        </div>
      </div>
      
      <div class="col-md-3">
        <div class="card text-white bg-success">
          <div class="card-body">
            <h2 class="card-title">{{ stats.available }}</h2>
            <p class="card-text">Available</p>
            <router-link to="/equipment" class="btn btn-light">View Available</router-link>
          </div>
        </div>
      </div>
      
      <div class="col-md-3">
        <div class="card text-white bg-warning">
          <div class="card-body">
            <h2 class="card-title">{{ stats.rented }}</h2>
            <p class="card-text">Currently Rented</p>
            <router-link to="/rentals" class="btn btn-light">View Rented</router-link>
          </div>
        </div>
      </div>
      
      <div class="col-md-3">
        <div class="card text-white bg-danger">
          <div class="card-body">
            <h2 class="card-title">{{ stats.overdue }}</h2>
            <p class="card-text">Overdue Rentals</p>
            <small v-if="!isAdmin">Admin Access Required</small>
            <router-link v-else to="/rentals?filter=overdue" class="btn btn-light">View Overdue</router-link>
          </div>
        </div>
      </div>
    </div>
    
    <div class="row">
      <!-- Quick Actions -->
      <div class="col-md-6">
        <div class="card">
          <div class="card-header">
            <h5>Quick Actions</h5>
          </div>
          <div class="card-body">
            <div class="d-grid gap-2">
              <button @click="showIssueModal = true" class="btn btn-primary">Issue Equipment</button>
              <button @click="showReturnModal = true" class="btn btn-success">Return Equipment</button>
              <router-link to="/rentals" class="btn btn-info">View My Rentals</router-link>
            </div>
          </div>
        </div>
      </div>
      
      <!-- System Status -->
      <div class="col-md-6">
        <div class="card">
          <div class="card-header">
            <h5>System Status</h5>
          </div>
          <div class="card-body">
            <p><strong>Active Rentals:</strong> {{ activeRentals.length }}</p>
            <p><strong>Overdue Rentals:</strong> {{ stats.overdue }}</p>
            <p><strong>Logged in as:</strong> {{ user?.name }} ({{ user?.role }})</p>
            <p><strong>System Status:</strong> <span class="text-success">Online</span></p>
          </div>
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
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { dashboardService } from '@/services/dashboard'
import IssueEquipmentModal from '@/components/IssueEquipmentModal.vue'
import ReturnEquipmentModal from '@/components/ReturnEquipmentModal.vue'

const authStore = useAuthStore()
const user = computed(() => authStore.user)
const isAdmin = computed(() => user.value?.role === 'Admin')

const stats = ref({
  total: 0,
  available: 0,
  rented: 0,
  overdue: 0
})

const activeRentals = ref([])
const showIssueModal = ref(false)
const showReturnModal = ref(false)

onMounted(async () => {
  await loadDashboardData()
})

const loadDashboardData = async () => {
  try {
    const [statsData, activeRentalsData] = await Promise.all([
      dashboardService.getEquipmentStats(),
      dashboardService.getActiveRentals()
    ])
    
    stats.value = statsData
    activeRentals.value = activeRentalsData
  } catch (error) {
    console.error('Failed to load dashboard data:', error)
  }
}

const handleIssueSuccess = () => {
  showIssueModal.value = false
  loadDashboardData()
}

const handleReturnSuccess = () => {
  showReturnModal.value = false
  loadDashboardData()
}
</script>
